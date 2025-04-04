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
        ASPNETCORE_ENVIRONMENT = 'Development'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Clean') {
            steps {
                script {
                    try {
                        sh '''
                            # Kill any existing dotnet processes using our ports
                            lsof -ti:${WEB_PORT} | xargs kill -9 || true
                            lsof -ti:${API_PORT} | xargs kill -9 || true
                        '''
                    } catch (Exception e) {
                        echo "Warning: Could not kill processes, they may not exist: ${e.message}"
                    }
                    
                    sh '''
                        dotnet clean || true
                        dotnet nuget locals all --clear || true
                    '''
                }
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
                    try {
                        // Start API in background with proper environment
                        sh """
                            cd ./publish/api
                            export ASPNETCORE_ENVIRONMENT=Development
                            export ASPNETCORE_URLS=http://localhost:${API_PORT}
                            nohup dotnet GroceryInventory.API.dll > api.log 2>&1 &
                            sleep 10
                        """
                        
                        // Start Web in background with proper environment
                        sh """
                            cd ./publish/web
                            export ASPNETCORE_ENVIRONMENT=Development
                            export ASPNETCORE_URLS=http://localhost:${WEB_PORT}
                            nohup dotnet GroceryInventory.Web.dll > web.log 2>&1 &
                            sleep 10
                        """
                        
                        // Give services time to fully start
                        sh 'sleep 10'
                    } catch (Exception e) {
                        echo "Warning during service startup: ${e.message}"
                        sh 'cat ./publish/api/api.log || true'
                        sh 'cat ./publish/web/web.log || true'
                        error "Failed to start services"
                    }
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    try {
                        // Check Web UI with retries
                        retry(3) {
                            sh """
                                curl -f http://localhost:${WEB_PORT} || (echo 'Web UI not responding' && exit 1)
                            """
                        }
                        
                        // Check API endpoints with retries
                        retry(3) {
                            sh """
                                curl -f http://localhost:${API_PORT}/api/groceryitems || (echo 'API not responding' && exit 1)
                            """
                        }
                    } catch (Exception e) {
                        echo "Health check failed: ${e.message}"
                        sh 'cat ./publish/api/api.log || true'
                        sh 'cat ./publish/web/web.log || true'
                        error "Health check failed"
                    }
                }
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
                
                // Archive logs if they exist
                archiveArtifacts artifacts: '**/publish/**/*.log', allowEmptyArchive: true
                
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
} 