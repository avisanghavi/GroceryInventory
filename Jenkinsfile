pipeline {
    agent any
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        NUGET_PACKAGES = '$(WORKSPACE)/.nuget/packages'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore') {
            steps {
                sh 'dotnet restore GroceryInventory.sln'
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build GroceryInventory.sln --no-restore'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test GroceryInventory.sln --no-build --verbosity normal'
            }
        }
        
        stage('Publish') {
            when {
                branch 'main'
            }
            steps {
                sh 'dotnet publish src/GroceryInventory.Web/GroceryInventory.Web.csproj -c Release -o ./publish/web'
                sh 'dotnet publish src/GroceryInventory.API/GroceryInventory.API.csproj -c Release -o ./publish/api'
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