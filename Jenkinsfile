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

        stage('Deploy to IIS') {
            steps {
                // FIX 3: Triple single-quotes treat everything literally, changing variable syntax to standard Windows %VAR%
                bat '''
                @echo off
                echo Stopping IIS application pool and site...
                %windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:"%IIS_APP_POOL%"
                %windir%\\system32\\inetsrv\\appcmd stop site /site.name:"%IIS_SITE%"

                echo Waiting for processes to release file locks...
                powershell -Command "Start-Sleep -Seconds 5"

                echo Deploying files to %DEPLOY_DIR%...
                if not exist "%DEPLOY_DIR%" mkdir "%DEPLOY_DIR%"
                
                :: FIX 4: Robocopy replaces xcopy to safely mirror the directories and remove old, deleted files
                robocopy "%PUBLISH_DIR%" "%DEPLOY_DIR%" /MIR /R:3 /W:5
                
                :: Robocopy exit codes 0-3 mean successful copy/no changes. Anything 8 or higher is a real error.
                if %ERRORLEVEL% GEQ 8 (
                    echo Robocopy failed with exit code %ERRORLEVEL%
                    exit /b %ERRORLEVEL%
                )

                echo Starting IIS application pool and site...
                %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:"%IIS_APP_POOL%"
                %windir%\\system32\\inetsrv\\appcmd start site /site.name:"%IIS_SITE%"
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