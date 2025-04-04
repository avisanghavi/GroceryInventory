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
                    echo "Current directory: $(pwd)"
                    echo "Directory contents:"
                    ls -la
                    echo ".NET SDK version:"
                    dotnet --version || true
                    echo ".NET SDK info:"
                    dotnet --info || true
                    echo "Environment variables:"
                    env | sort
                '''
            }
        }
        
        stage('Checkout') {
            steps {
                echo "Checking out code from SCM"
                checkout scm
                sh '''
                    echo "Workspace contents after checkout:"
                    ls -la
                    echo "Solution file check:"
                    find . -name "*.sln" -type f
                '''
            }
        }
        
        stage('Clean') {
            steps {
                sh '''
                    echo "Cleaning solution..."
                    echo "Current directory: $(pwd)"
                    
                    # Find solution file
                    SOLUTION_FILE=$(find . -name "*.sln" -type f)
                    if [ -n "$SOLUTION_FILE" ]; then
                        echo "Found solution file: $SOLUTION_FILE"
                        dotnet clean "$SOLUTION_FILE" --verbosity detailed || true
                    else
                        echo "Solution file not found in: $(pwd)"
                        ls -la
                        exit 1
                    fi
                    
                    echo "Cleaning bin and obj folders..."
                    find . -type d -name 'bin' -o -name 'obj' | while read dir; do
                        echo "Removing $dir"
                        rm -rf "$dir"
                    done
                '''
            }
        }
        
        stage('Restore') {
            steps {
                sh '''
                    echo "Clearing NuGet cache..."
                    dotnet nuget locals all --clear
                    
                    echo "Restoring packages..."
                    SOLUTION_FILE=$(find . -name "*.sln" -type f)
                    if [ -n "$SOLUTION_FILE" ]; then
                        echo "Found solution file: $SOLUTION_FILE"
                        dotnet restore "$SOLUTION_FILE" --verbosity detailed
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
                    SOLUTION_FILE=$(find . -name "*.sln" -type f)
                    if [ -n "$SOLUTION_FILE" ]; then
                        echo "Found solution file: $SOLUTION_FILE"
                        dotnet build "$SOLUTION_FILE" --configuration Release --no-restore --verbosity detailed
                    else
                        echo "Solution file not found"
                        exit 1
                    fi
                '''
            }
        }
        
        stage('Test') {
            steps {
                sh '''
                    echo "Running tests..."
                    SOLUTION_FILE=$(find . -name "*.sln" -type f)
                    if [ -n "$SOLUTION_FILE" ]; then
                        echo "Found solution file: $SOLUTION_FILE"
                        dotnet test "$SOLUTION_FILE" --configuration Release --no-build --verbosity detailed
                    else
                        echo "Solution file not found"
                        exit 1
                    fi
                '''
            }
        }
        
        stage('Publish') {
            steps {
                sh '''
                    echo "Publishing projects..."
                    rm -rf ./publish || true
                    mkdir -p ./publish
                    
                    # Function to publish a project
                    publish_project() {
                        local project_path="$1"
                        local output_path="$2"
                        if [ -f "$project_path" ]; then
                            echo "Publishing project: $project_path to $output_path"
                            dotnet publish "$project_path" --configuration Release --output "$output_path" --verbosity detailed
                        else
                            echo "Project not found: $project_path"
                            ls -la $(dirname "$project_path")
                            return 1
                        fi
                    }
                    
                    # Publish Web project
                    WEB_PROJECT="src/GroceryInventory.Web/GroceryInventory.Web.csproj"
                    publish_project "$WEB_PROJECT" "./publish/web"
                    
                    # Publish API project
                    API_PROJECT="src/GroceryInventory.API/GroceryInventory.API.csproj"
                    publish_project "$API_PROJECT" "./publish/api"
                '''
            }
        }
    }
    
    post {
        always {
            echo "Build completed - cleaning up workspace"
            script {
                sh '''
                    echo "Workspace contents before cleanup:"
                    ls -la
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
            echo "Build failed. Please check the logs above for details."
            sh '''
                echo "Environment state at failure:"
                echo "Current directory: $(pwd)"
                echo "Directory contents:"
                ls -la
                echo ".NET SDK version:"
                dotnet --version || true
            '''
        }
    }
} 