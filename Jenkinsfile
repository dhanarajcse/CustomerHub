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
                    echo "Streaming compiled files over port 80 via Web Deploy Agent Service..."
                    
                    "C:\\Program Files\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe" ^
                    -source:contentPath="%WORKSPACE%\\%PUBLISH_DIR%" ^
                    -dest:contentPath="CustomerHub",computerName="http://16.171.14.84/MSDEPLOYAGENTSERVICE",username="%EC2_USER%",password="%EC2_PASS%",authType="NTLM" ^
                    -verb:sync ^
                    -allowUntrusted
                    """
                }
            }
        }
    } // <-- FIX: This closing brace for the 'stages' block was missing!

    post {
        success {
            echo 'CustomerHub deployed to IIS successfully.'
        }
        failure {
            echo 'CustomerHub deployment failed. Check the console output.'
        }
    }
}