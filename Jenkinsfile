pipeline {
    agent any
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        NUGET_PACKAGES = '$(WORKSPACE)/.nuget/packages'
        DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
        DOTNET_VERSION = '7.0'  // Adjust this based on your .NET version
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
                sh 'dotnet restore ${SOLUTION_FILE}'
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build ${SOLUTION_FILE} --configuration Release --no-restore'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test ${SOLUTION_FILE} --no-restore --verbosity normal'
            }
            post {
                always {
                    junit '**/TestResults/*.xml'
                }
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
        
        stage('Health Check') {
            steps {
                script {
                    try {
                        // Check API endpoints
                        sh 'curl -f http://localhost:5003/api/groceryitems'
                        sh 'curl -f http://localhost:5003/api/suppliers'
                        sh 'curl -f http://localhost:5003/api/orders'
                        
                        // Check Web UI
                        sh 'curl -f http://localhost:5002'
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
            cleanWs()
        }
        success {
            echo 'Pipeline completed successfully!'
        }
        failure {
            echo 'Pipeline failed!'
        }
    }
} 