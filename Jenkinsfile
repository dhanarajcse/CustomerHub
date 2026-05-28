pipeline {
    agent any

    // FORCE WEBHOOK REGISTRY: This tells the Jenkins engine to listen to GitHub pushes automatically
    triggers {
        githubPush()
    }

    environment {
        BUILD_CONFIGURATION = 'Release'
        PUBLISH_DIR         = 'publish'
        // FIX 1: Double backslashes inside double quotes escape to single backslashes correctly
        DEPLOY_DIR          = "C:\\inetpub\\wwwroot\\CustomerHub"
        IIS_SITE            = 'CustomerHub'
        IIS_APP_POOL        = 'CustomerHub'
    }

    stages {
        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build --configuration %BUILD_CONFIGURATION% --no-restore'
            }
        }

        stage('Publish') {
            steps {
                // FIX 2: Added a clean step to prevent appending old files to new deployments
                bat 'if exist %PUBLISH_DIR% rmdir /S /Q %PUBLISH_DIR%'
                bat 'dotnet publish --configuration %BUILD_CONFIGURATION% --output %PUBLISH_DIR% --no-build'
            }
        }

        stage('Deploy to EC2 IIS') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'ec2-iis-credentials', passwordVariable: 'EC2_PASS', usernameVariable: 'EC2_USER')]) {
                    powershell """
                    # Create secure credentials object
                    \$secpasswd = ConvertTo-SecureString '${EC2_PASS}' -AsPlainText -Force
                    \$mycreds = New-Object System.Management.Automation.PSCredential ("16.171.14.84\\${EC2_USER}", \$secpasswd)
                    
                    # Create a remote session directly to your AWS EC2 instance over Port 5985
                    \$session = New-PSSession -ComputerName "16.171.14.84" -Credential \$mycreds -Authentication Negotiate
                    
                    Write-Output "Stopping IIS App Pool on EC2 via WinRM..."
                    Invoke-Command -Session \$session -ScriptBlock { 
                        New-Item -Path "C:\\inetpub\\wwwroot\\CustomerHub\\app_offline.htm" -ItemType File -Value "AppOffline" -Force 
                    }
                    
                    Write-Output "Shipping deployment binaries over secure session..."
                    Copy-Item -Path "$WORKSPACE\\$PUBLISH_DIR\\*" -Destination "C:\\inetpub\\wwwroot\\CustomerHub\\" -ToSession \$session -Recurse -Force
                    
                    Write-Output "Bringing IIS Web App back online and recycling pool..."
                    Invoke-Command -Session \$session -ScriptBlock { 
                        if (Test-Path "C:\\inetpub\\wwwroot\\CustomerHub\\app_offline.htm") { 
                            Remove-Item "C:\\inetpub\\wwwroot\\CustomerHub\\app_offline.htm" -Force 
                        } 
                        
                        # FORCE RECYCLE: Clears out stale process memory to avoid 500 errors
                        Import-Module WebAdministration
                        Restart-WebAppPool -Name "CustomerHub"
                    }
                    
                    # Clean up active session
                    Remove-PSSession \$session
                    """
                }
            }
        }

    } 

    post {
        success {
            echo 'CustomerHub deployed to IIS successfully.'
        }
        failure {
            echo 'CustomerHub deployment failed. Check the console output.'
        }
    }
}