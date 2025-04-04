pipeline {
    agent any
    
    tools {
        dotnetsdk 'dotnet-7.0'
    }
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        ASPNETCORE_ENVIRONMENT = 'Development'
    }
    
    stages {
        stage('Setup') {
            steps {
                echo "Using .NET SDK from tools configuration"
                sh '''
                    dotnet --version
                    dotnet --info
                '''
            }
        }
        
        stage('Checkout') {
            steps {
                checkout scm
                sh 'ls -la'  // Show workspace contents
            }
        }
        
        stage('Clean') {
            steps {
                sh '''
                    echo "Cleaning solution..."
                    if [ -f "GroceryInventory.sln" ]; then
                        dotnet clean GroceryInventory.sln || true
                    else
                        echo "Solution file not found in: $(pwd)"
                        ls -la
                    fi
                    
                    echo "Cleaning bin and obj folders..."
                    find . -type d -name 'bin' -o -name 'obj' | xargs rm -rf
                '''
            }
        }
        
        stage('Restore') {
            steps {
                sh '''
                    echo "Clearing NuGet cache..."
                    dotnet nuget locals all --clear
                    
                    echo "Restoring packages..."
                    if [ -f "GroceryInventory.sln" ]; then
                        dotnet restore GroceryInventory.sln --verbosity normal
                    else
                        echo "Solution file not found in: $(pwd)"
                        ls -la
                        exit 1
                    fi
                '''
            }
        }
        
        stage('Build') {
            steps {
                sh '''
                    echo "Building solution..."
                    dotnet build GroceryInventory.sln --configuration Release --no-restore --verbosity normal
                '''
            }
        }
        
        stage('Test') {
            steps {
                sh '''
                    echo "Running tests..."
                    dotnet test GroceryInventory.sln --configuration Release --no-build --verbosity normal
                '''
            }
        }
        
        stage('Publish') {
            steps {
                sh '''
                    echo "Publishing projects..."
                    rm -rf ./publish || true
                    
                    echo "Publishing Web project..."
                    if [ -f "src/GroceryInventory.Web/GroceryInventory.Web.csproj" ]; then
                        dotnet publish src/GroceryInventory.Web/GroceryInventory.Web.csproj --configuration Release --output ./publish/web --no-restore
                    else
                        echo "Web project not found!"
                        ls -la src/
                        exit 1
                    fi
                    
                    echo "Publishing API project..."
                    if [ -f "src/GroceryInventory.API/GroceryInventory.API.csproj" ]; then
                        dotnet publish src/GroceryInventory.API/GroceryInventory.API.csproj --configuration Release --output ./publish/api --no-restore
                    else
                        echo "API project not found!"
                        ls -la src/
                        exit 1
                    fi
                '''
            }
        }
    }
    
    post {
        always {
            script {
                sh '''
                    echo "Cleaning up..."
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