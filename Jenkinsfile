pipeline {
    agent any

    environment {
        BUILD_CONFIGURATION = 'Release'
        PUBLISH_DIR = 'publish'
        DEPLOY_DIR = 'C:\\inetpub\\wwwroot\\CustomerHub'
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

        stage('Deploy to IIS') {
            steps {
                bat '''
                %windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:"CustomerHub"
                %windir%\\system32\\inetsrv\\appcmd stop site /site.name:"CustomerHub"

                powershell -Command "Start-Sleep -Seconds 5"

                if not exist "%DEPLOY_DIR%" mkdir "%DEPLOY_DIR%"
                xcopy /E /Y /I "%PUBLISH_DIR%" "%DEPLOY_DIR%"

                %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:"CustomerHub"
                %windir%\\system32\\inetsrv\\appcmd start site /site.name:"CustomerHub"
                '''
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