# Infrastructure Setup

This directory contains the infrastructure automation and deployment configuration for the Grocery Inventory System.

## Directory Structure

```
infrastructure/
├── ansible/
│   ├── roles/                  # Ansible roles
│   ├── inventory/              # Environment inventories
│   │   ├── production/         # Production environment
│   │   └── staging/            # Staging environment
│   ├── group_vars/             # Group variables
│   ├── host_vars/              # Host-specific variables
│   └── deploy.yml              # Main deployment playbook
└── jenkins/                    # Jenkins pipeline configuration
    └── Jenkinsfile             # Pipeline definition
```

## Prerequisites

1. Ansible 2.9 or later
2. Jenkins with the following plugins:
   - Ansible plugin
   - Pipeline plugin
   - Git plugin
3. PostgreSQL 13 or later
4. .NET 7.0 SDK and Runtime
5. Nginx

## Setup Instructions

### Ansible Setup

1. Install required Ansible roles:

```bash
ansible-galaxy install -r requirements.yml
```

2. Create an Ansible vault for sensitive data:

```bash
ansible-vault create group_vars/vault.yml
```

3. Add the following variables to the vault:

```yaml
vault_db_password: your_secure_password
vault_ssl_cert: your_ssl_certificate
vault_ssl_key: your_ssl_private_key
```

### Jenkins Setup

1. Create a new pipeline job in Jenkins
2. Configure the job to use the `Jenkinsfile` from the repository
3. Add the following credentials to Jenkins:
   - Ansible Vault Key
   - SSH Deploy Key
   - Database Credentials

### Deployment

1. For staging:

```bash
ansible-playbook -i inventory/staging deploy.yml
```

2. For production:

```bash
ansible-playbook -i inventory/production deploy.yml
```

## Security Considerations

1. All sensitive data is stored in Ansible Vault
2. SSL/TLS is enforced in production
3. Security headers are enabled by default
4. Database connections are restricted to internal network
5. Regular security updates are applied automatically

## Monitoring

The infrastructure includes:

- Application logging via systemd journal
- Nginx access and error logs
- PostgreSQL query logging
- System metrics collection

## Maintenance

### Database Backups

Daily automated backups are configured:

```bash
ansible-playbook -i inventory/production backup.yml
```

### Updates

To update the system:

```bash
ansible-playbook -i inventory/production update.yml
```

## Troubleshooting

Common issues and solutions:

1. **Database Connection Issues**

   - Check PostgreSQL service status
   - Verify connection string in appsettings.json
   - Check firewall rules

2. **Application Deployment Failures**

   - Check Jenkins build logs
   - Verify Ansible playbook execution
   - Check systemd service status

3. **Nginx Configuration**
   - Test configuration: `nginx -t`
   - Check error logs: `journalctl -u nginx`
   - Verify SSL certificates

## Support

For infrastructure support, contact:

- Email: infrastructure-support@example.com
- Slack: #infrastructure-support
