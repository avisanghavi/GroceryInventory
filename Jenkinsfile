pipeline {
    agent any
    
    environment {
        DOTNET_VERSION = '7.0'
        PROJECT_NAME = 'GroceryInventory'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Clean') {
            steps {
                sh 'dotnet clean'
            }
        }
        
        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test --no-restore --verbosity normal'
            }
        }
        
        stage('Publish') {
            steps {
                sh '''
                    dotnet publish src/GroceryInventory.Web/GroceryInventory.Web.csproj --configuration Release --output ./publish/web
                    dotnet publish src/GroceryInventory.API/GroceryInventory.API.csproj --configuration Release --output ./publish/api
                '''
            }
        }
    }
    
    post {
        always {
            cleanWs()
        }
        success {
            echo "Build completed successfully!"
        }
        failure {
            echo "Build failed. Please check the logs for details."
        }
    }
} 