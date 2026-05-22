pipeline {
    agent any

    environment {
        BUILD_CONFIGURATION = 'Release'
        PUBLISH_DIR = 'publish'
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
                bat 'dotnet publish --configuration %BUILD_CONFIGURATION% --output %PUBLISH_DIR% --no-build'
            }
        }
    }

    post {
        success {
            echo 'CustomerHub build and publish completed successfully.'
        }

        failure {
            echo 'CustomerHub pipeline failed. Check the console output.'
        }
    }
}