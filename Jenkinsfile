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
                    bat """
                    @echo off
                    echo "Force clearing existing network caches to prevent ghost connections..."
                    net use * /delete /y >nul 2>&1
                    
                    echo "Authenticating fresh pipeline session with explicit IP domain routing..."
                    net use \\\\16.171.14.84\\CustomerHubShare "%EC2_PASS%" /user:16.171.14.84\\"%EC2_USER%" /persistent:no
                    
                    echo "Mirroring compiled application directory directly onto AWS EC2 IIS..."
                    robocopy "%WORKSPACE%\\%PUBLISH_DIR%" "\\\\16.171.14.84\\CustomerHubShare" /MIR /R:3 /W:5
                    
                    echo "Cleaning up active network pipeline connections..."
                    net use \\\\16.171.14.84\\CustomerHubShare /delete /y
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