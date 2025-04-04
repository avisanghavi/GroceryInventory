pipeline {
    agent any
    
    environment {
        DOTNET_VERSION = '7.0'
        PROJECT_NAME = 'GroceryInventory'
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        ASPNETCORE_ENVIRONMENT = 'Development'
    }
    
    stages {
        stage('Setup') {
            steps {
                script {
                    // Check .NET version
                    sh '''
                        dotnet --version
                        dotnet --info
                    '''
                }
            }
        }
        
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Clean') {
            steps {
                sh '''
                    dotnet clean GroceryInventory.sln || true
                    rm -rf */bin */obj || true
                '''
            }
        }
        
        stage('Restore') {
            steps {
                sh '''
                    dotnet nuget locals all --clear
                    dotnet restore GroceryInventory.sln
                '''
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build GroceryInventory.sln --configuration Release --no-restore'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test GroceryInventory.sln --configuration Release --no-build --verbosity normal'
            }
        }
        
        stage('Publish') {
            steps {
                sh '''
                    rm -rf ./publish || true
                    dotnet publish src/GroceryInventory.Web/GroceryInventory.Web.csproj --configuration Release --output ./publish/web --no-restore
                    dotnet publish src/GroceryInventory.API/GroceryInventory.API.csproj --configuration Release --output ./publish/api --no-restore
                '''
            }
        }
    }
    
    post {
        always {
            script {
                sh '''
                    rm -rf ./publish || true
                    dotnet nuget locals all --clear || true
                '''
                cleanWs()
            }
        }
        success {
            echo "Build completed successfully!"
        }
        failure {
            echo "Build failed. Please check the logs for details."
        }
    }
} 