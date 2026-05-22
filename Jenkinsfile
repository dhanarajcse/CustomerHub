pipeline {
    agent any

    environment {
        PROJECT_NAME = 'CustomerHub'
        PUBLISH_DIR = 'publish'
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/YOUR_USERNAME/CustomerHub.git'
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release --output publish'
            }
        }

        stage('Deploy') {
            steps {
                bat '''
                if not exist C:\\inetpub\\wwwroot\\CustomerHub mkdir C:\\inetpub\\wwwroot\\CustomerHub
                xcopy /E /Y /I publish C:\\inetpub\\wwwroot\\CustomerHub
                '''
            }
        }
    }
}