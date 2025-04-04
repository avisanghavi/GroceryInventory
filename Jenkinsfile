pipeline {
    agent any
    
    environment {
        DOTNET_VERSION = '7.0'
        PROJECT_NAME = 'GroceryInventory'
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 'true'
        ASPNETCORE_ENVIRONMENT = 'Development'
        DOTNET_ROOT = "/usr/local/share/dotnet"
        PATH = "${DOTNET_ROOT}:/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin:${PATH}"
    }
    
    stages {
        stage('Setup') {
            steps {
                script {
                    // Install .NET SDK if not present
                    sh '''
                        echo "Current PATH: $PATH"
                        echo "Current directory: $(pwd)"
                        
                        # Create installation directory if it doesn't exist
                        sudo mkdir -p ${DOTNET_ROOT}
                        sudo chown -R $(whoami) ${DOTNET_ROOT}
                        
                        # Check if dotnet is installed
                        if ! command -v dotnet &> /dev/null; then
                            echo ".NET SDK not found. Installing..."
                            
                            # Download the install script
                            curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
                            chmod +x dotnet-install.sh
                            
                            # Install .NET SDK
                            ./dotnet-install.sh --version ${DOTNET_VERSION} --install-dir ${DOTNET_ROOT}
                            
                            # Cleanup
                            rm dotnet-install.sh
                        else
                            echo "Found existing dotnet installation:"
                            which dotnet
                        fi
                        
                        # Verify installation
                        echo "Verifying .NET installation..."
                        export PATH="${DOTNET_ROOT}:$PATH"
                        dotnet --version
                        dotnet --info
                    '''
                }
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