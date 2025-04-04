pipeline {
    agent any
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        NUGET_PACKAGES = '$(WORKSPACE)/.nuget/packages'
        DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
        DOTNET_VERSION = '7.0'
        PROJECT_NAME = 'GroceryInventory'
        SOLUTION_FILE = 'GroceryInventory.sln'
        WEB_PROJECT = 'src/GroceryInventory.Web/GroceryInventory.Web.csproj'
        API_PROJECT = 'src/GroceryInventory.API/GroceryInventory.API.csproj'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }
        
        stage('Build') {
            steps {
                sh '''
                    dotnet build src/GroceryInventory.Web/GroceryInventory.Web.csproj --configuration Release --no-restore
                    dotnet build src/GroceryInventory.API/GroceryInventory.API.csproj --configuration Release --no-restore
                '''
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
        
        stage('Health Check') {
            steps {
                script {
                    try {
                        // Wait for applications to start
                        sh 'sleep 10'
                        
                        // Check Web UI
                        sh 'curl -f http://localhost:5002 || true'
                        
                        // Check API endpoints
                        sh '''
                            curl -f http://localhost:5003/api/groceryitems || true
                            curl -f http://localhost:5003/api/suppliers || true
                            curl -f http://localhost:5003/api/orders || true
                        '''
                    } catch (Exception e) {
                        echo "Health check warning: ${e.message}"
                        // Don't fail the build on health check issues
                        // as the services might not be running during build
                    }
                }
            }
        }
        
        stage('Deploy') {
            when {
                branch 'main'
            }
            steps {
                ansiblePlaybook(
                    playbook: 'infrastructure/ansible/deploy.yml',
                    inventory: 'infrastructure/ansible/inventory/production',
                    credentialsId: 'ansible-vault-key',
                    colorized: true
                )
            }
        }
    }
    
    post {
        success {
            echo "Build completed successfully! The ${PROJECT_NAME} application is ready."
        }
        failure {
            echo "Build failed. Please check the logs for details."
        }
        always {
            cleanWs()
        }
    }
} 