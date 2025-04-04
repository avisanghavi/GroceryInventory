pipeline {
    agent any
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        NUGET_PACKAGES = '${WORKSPACE}/.nuget/packages'
        DOTNET_CLI_HOME = '/tmp/DOTNET_CLI_HOME'
        DOTNET_VERSION = '7.0'
        PROJECT_NAME = 'GroceryInventory'
        SOLUTION_FILE = './GroceryInventory.sln'
        WEB_PROJECT = './src/GroceryInventory.Web/GroceryInventory.Web.csproj'
        API_PROJECT = './src/GroceryInventory.API/GroceryInventory.API.csproj'
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
                            
                            # Clean solution and NuGet cache
                            dotnet clean "${SOLUTION_FILE}" || true
                            dotnet nuget locals all --clear || true
                            
                            # Remove any existing publish directories
                            rm -rf ./publish || true
                        '''
                    } catch (Exception e) {
                        echo "Warning: Clean stage had issues: ${e.message}"
                        // Continue pipeline despite clean issues
                    }
                }
            }
        }
        
        stage('Restore Dependencies') {
            steps {
                script {
                    try {
                        sh """
                            # Ensure solution exists
                            if [ ! -f "${SOLUTION_FILE}" ]; then
                                echo "Solution file not found at ${SOLUTION_FILE}"
                                exit 1
                            fi
                            
                            dotnet restore "${SOLUTION_FILE}"
                        """
                    } catch (Exception e) {
                        error "Failed to restore dependencies: ${e.message}"
                    }
                }
            }
        }
        
        stage('Build') {
            steps {
                script {
                    try {
                        sh """
                            dotnet build "${SOLUTION_FILE}" --configuration Release --no-restore
                        """
                    } catch (Exception e) {
                        error "Build failed: ${e.message}"
                    }
                }
            }
        }
        
        stage('Test') {
            steps {
                script {
                    try {
                        sh """
                            dotnet test "${SOLUTION_FILE}" --no-restore --verbosity normal
                        """
                    } catch (Exception e) {
                        error "Tests failed: ${e.message}"
                    }
                }
            }
        }
        
        stage('Publish') {
            steps {
                script {
                    try {
                        sh """
                            # Ensure project files exist
                            if [ ! -f "${WEB_PROJECT}" ]; then
                                echo "Web project file not found at ${WEB_PROJECT}"
                                exit 1
                            fi
                            if [ ! -f "${API_PROJECT}" ]; then
                                echo "API project file not found at ${API_PROJECT}"
                                exit 1
                            fi
                            
                            dotnet publish "${WEB_PROJECT}" --configuration Release --output ./publish/web --no-restore
                            dotnet publish "${API_PROJECT}" --configuration Release --output ./publish/api --no-restore
                        """
                    } catch (Exception e) {
                        error "Publish failed: ${e.message}"
                    }
                }
            }
        }
        
        stage('Start Services') {
            steps {
                script {
                    try {
                        // Create directories for logs
                        sh 'mkdir -p ./publish/api/logs ./publish/web/logs'
                        
                        // Start API in background with proper environment
                        sh """
                            cd ./publish/api
                            export ASPNETCORE_ENVIRONMENT=Development
                            export ASPNETCORE_URLS="http://localhost:${API_PORT}"
                            nohup dotnet GroceryInventory.API.dll > logs/api.log 2>&1 &
                            echo \$! > ./api.pid
                            sleep 15
                            
                            # Verify API process is running
                            if ! ps -p \$(cat ./api.pid) > /dev/null; then
                                echo "API process failed to start"
                                cat logs/api.log
                                exit 1
                            fi
                        """
                        
                        // Start Web in background with proper environment
                        sh """
                            cd ./publish/web
                            export ASPNETCORE_ENVIRONMENT=Development
                            export ASPNETCORE_URLS="http://localhost:${WEB_PORT}"
                            nohup dotnet GroceryInventory.Web.dll > logs/web.log 2>&1 &
                            echo \$! > ./web.pid
                            sleep 15
                            
                            # Verify Web process is running
                            if ! ps -p \$(cat ./web.pid) > /dev/null; then
                                echo "Web process failed to start"
                                cat logs/web.log
                                exit 1
                            fi
                        """
                    } catch (Exception e) {
                        echo "Warning during service startup: ${e.message}"
                        sh '''
                            cat ./publish/api/logs/api.log || true
                            cat ./publish/web/logs/web.log || true
                        '''
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
                                sleep 5
                                curl -f -v http://localhost:${WEB_PORT} || (echo 'Web UI not responding' && exit 1)
                            """
                        }
                        
                        // Check API endpoints with retries
                        retry(3) {
                            sh """
                                sleep 5
                                curl -f -v http://localhost:${API_PORT}/api/groceryitems || (echo 'API not responding' && exit 1)
                            """
                        }
                    } catch (Exception e) {
                        echo "Health check failed: ${e.message}"
                        sh '''
                            cat ./publish/api/logs/api.log || true
                            cat ./publish/web/logs/web.log || true
                            
                            # Print running processes on these ports
                            lsof -i :${WEB_PORT} || true
                            lsof -i :${API_PORT} || true
                        '''
                        error "Health check failed"
                    }
                }
            }
        }
    }
    
    post {
        always {
            script {
                // Stop services using PID files if they exist
                sh '''
                    if [ -f ./publish/api/api.pid ]; then
                        kill -9 `cat ./publish/api/api.pid` || true
                        rm ./publish/api/api.pid
                    fi
                    
                    if [ -f ./publish/web/web.pid ]; then
                        kill -9 `cat ./publish/web/web.pid` || true
                        rm ./publish/web/web.pid
                    fi
                    
                    # Backup measure to kill any remaining processes
                    lsof -ti:${WEB_PORT} | xargs kill -9 || true
                    lsof -ti:${API_PORT} | xargs kill -9 || true
                '''
                
                // Archive logs if they exist
                archiveArtifacts artifacts: '**/publish/**/logs/*.log', allowEmptyArchive: true
                
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