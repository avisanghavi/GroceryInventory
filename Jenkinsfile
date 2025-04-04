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
        WEB_PORT = '5012'
        API_PORT = '5013'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Clean') {
            steps {
                sh '''
                    # Kill any existing dotnet processes using our ports
                    lsof -ti:${WEB_PORT} | xargs kill -9 || true
                    lsof -ti:${API_PORT} | xargs kill -9 || true
                    
                    dotnet clean
                    dotnet nuget locals all --clear
                '''
            }
        }
        
        stage('Restore Dependencies') {
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
                    dotnet publish ${WEB_PROJECT} --configuration Release --output ./publish/web
                    dotnet publish ${API_PROJECT} --configuration Release --output ./publish/api
                '''
            }
        }
        
        stage('Start Services') {
            steps {
                script {
                    // Start API in background
                    sh """
                        cd ./publish/api
                        ASPNETCORE_URLS=http://localhost:${API_PORT} dotnet GroceryInventory.API.dll &
                        sleep 5
                    """
                    
                    // Start Web in background
                    sh """
                        cd ./publish/web
                        ASPNETCORE_URLS=http://localhost:${WEB_PORT} dotnet GroceryInventory.Web.dll &
                        sleep 5
                    """
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    try {
                        // Check Web UI
                        sh "curl -f http://localhost:${WEB_PORT}"
                        
                        // Check API endpoints
                        sh """
                            curl -f http://localhost:${API_PORT}/api/groceryitems
                            curl -f http://localhost:${API_PORT}/api/suppliers
                            curl -f http://localhost:${API_PORT}/api/orders
                        """
                    } catch (Exception e) {
                        error "Health check failed: ${e.message}"
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
        always {
            script {
                // Stop any running services
                sh """
                    lsof -ti:${WEB_PORT} | xargs kill -9 || true
                    lsof -ti:${API_PORT} | xargs kill -9 || true
                """
                cleanWs()
            }
        }
        success {
            echo "Build completed successfully! The ${PROJECT_NAME} application is ready."
        }
        failure {
            echo "Build failed. Please check the logs for details."
        }
    }
} 