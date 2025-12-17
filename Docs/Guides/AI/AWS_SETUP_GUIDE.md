# ☁️ AWS Setup & Deployment Guide

**Complete AWS Configuration for MyVocaList AI Services**  
**Cost: $0 for 12 months | Time: 2-3 hours initial setup**

---

## 🎯 What You'll Build

```
Your AWS Architecture:
┌────────────────────────────────────────────┐
│         API Gateway (REST API)             │
│   https://your-api.execute-api.region...  │
└──────────────┬─────────────────────────────┘
               │
    ┌──────────┴──────────┬──────────────┐
    │                     │              │
┌───▼────┐          ┌────▼────┐    ┌───▼────┐
│Lambda 1│          │Lambda 2 │    │Lambda 3│
│Pitch   │          │Recommend│    │Lyrics  │
└───┬────┘          └────┬────┘    └───┬────┘
    │                    │             │
    └──────────┬─────────┴─────────────┘
               │
         ┌─────▼──────┐
         │ S3 Bucket  │
         │Audio files │
         └────────────┘
```

---

## 📋 Prerequisites

### Before You Start
- [ ] AWS account created (https://aws.amazon.com/free/)
- [ ] Credit/debit card for verification (won't be charged)
- [ ] Python 3.11 installed locally
- [ ] Basic command line familiarity

### Tools to Install
- [ ] AWS CLI
- [ ] AWS SAM CLI
- [ ] Docker (for local testing)

---

## 🔧 Part 1: AWS Account Setup (30 minutes)

### Step 1: Create AWS Account

```bash
1. Go to https://aws.amazon.com/free/
2. Click "Create a Free Account"
3. Fill in:
   - Email: your-email@example.com
   - Password: strong-password
   - Account name: MyVocaList-Dev

4. Contact Information:
   - Choose: Professional
   - Company: MyVocaList / Personal Project
   - Phone: your-phone

5. Payment Method:
   - Add card (for verification only)
   - Won't be charged if staying in free tier
   
6. Identity Verification:
   - Receive code via SMS
   - Enter code

7. Select Support Plan:
   - Choose: Basic (Free)

✅ Account created!
```

---

### Step 2: Secure Your Account

```bash
# Enable MFA (Multi-Factor Authentication)
1. Go to: IAM Dashboard > Security Credentials
2. Click "Assign MFA device"
3. Choose: Virtual MFA device
4. Use app: Google Authenticator, Authy, or Microsoft Authenticator
5. Scan QR code
6. Enter two consecutive codes

✅ Root account secured!
```

---

### Step 3: Create IAM User (Don't use root!)

```bash
# Best practice: Never use root account for daily work

1. Go to: IAM > Users > Add users
2. User name: myvocalist-admin
3. Access type: ✅ Programmatic access
               ✅ AWS Management Console access
4. Console password: Custom password
5. ✅ Require password reset (optional)
6. Click Next

7. Permissions:
   - Attach existing policies directly
   - Select: AdministratorAccess (for learning)
   
   # In production, use least privilege!
   # For now, full access makes learning easier

8. Tags (optional):
   - Key: Project, Value: MyVocaList
   - Key: Environment, Value: Development

9. Review and Create

10. IMPORTANT: Download CSV with credentials!
    - Access key ID
    - Secret access key
    - Console login link

✅ IAM user created!
```

---

### Step 4: Install AWS CLI

**Windows:**
```powershell
# Download installer
https://awscli.amazonaws.com/AWSCLIV2.msi

# Run installer
# Verify
aws --version
# Should show: aws-cli/2.x.x
```

**macOS:**
```bash
# Using Homebrew
brew install awscli

# Or download installer
curl "https://awscli.amazonaws.com/AWSCLIV2.pkg" -o "AWSCLIV2.pkg"
sudo installer -pkg AWSCLIV2.pkg -target /

# Verify
aws --version
```

**Linux:**
```bash
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip
sudo ./aws/install

# Verify
aws --version
```

---

### Step 5: Configure AWS CLI

```bash
# Configure with your IAM user credentials
aws configure

# Enter when prompted:
AWS Access Key ID: [from downloaded CSV]
AWS Secret Access Key: [from downloaded CSV]
Default region name: us-east-1  # or sa-east-1 for Brazil
Default output format: json

# Test connection
aws sts get-caller-identity

# Should return:
{
    "UserId": "AIDAI...",
    "Account": "123456789012",
    "Arn": "arn:aws:iam::123456789012:user/myvocalist-admin"
}

✅ AWS CLI configured!
```

---

## 🚀 Part 2: Install AWS SAM CLI (30 minutes)

SAM = Serverless Application Model (makes Lambda deployment easy!)

### Windows Installation

```powershell
# Download installer
https://github.com/aws/aws-sam-cli/releases/latest/download/AWS_SAM_CLI_64_PY3.msi

# Run installer
# Verify
sam --version
# Should show: SAM CLI, version 1.x.x
```

### macOS Installation

```bash
# Using Homebrew
brew tap aws/tap
brew install aws-sam-cli

# Verify
sam --version
```

### Linux Installation

```bash
# Download binary
wget https://github.com/aws/aws-sam-cli/releases/latest/download/aws-sam-cli-linux-x86_64.zip
unzip aws-sam-cli-linux-x86_64.zip -d sam-installation
sudo ./sam-installation/install

# Verify
sam --version
```

---

## 🏗️ Part 3: Create First Lambda Function (45 minutes)

### Project Structure

```bash
# Create project directory
mkdir myvocalist-pitch-api
cd myvocalist-pitch-api

# Initialize SAM project
sam init

# Select:
# 1 - AWS Quick Start Templates
# 1 - Hello World Example
# N - Use the most popular runtime? (Python 3.9)
# 11 - python3.11
# N - X-Ray tracing
# N - CloudWatch Insights
# Project name: myvocalist-pitch-api

✅ SAM project created!
```

### Project Structure Explained

```
myvocalist-pitch-api/
├── template.yaml          # AWS resources definition (Infrastructure as Code)
├── requirements.txt       # Python dependencies
├── hello_world/           # Lambda function folder
│   ├── __init__.py
│   ├── app.py            # Lambda handler (your code here!)
│   └── requirements.txt  # Function-specific dependencies
├── events/               # Test events for local testing
│   └── event.json
└── tests/                # Unit tests
    └── unit/
        └── test_handler.py
```

---

### Understanding `template.yaml`

```yaml
# This file defines your entire infrastructure!
# SAM converts this to CloudFormation (AWS's infrastructure language)

AWSTemplateFormatVersion: '2010-09-09'
Transform: AWS::Serverless-2016-10-31  # SAM magic!
Description: MyVocaList Pitch Analysis API

# Global settings for all functions
Globals:
  Function:
    Timeout: 30              # Max execution time
    MemorySize: 512          # RAM (128-10240 MB)
    Runtime: python3.11      # Python version
    Architectures:
      - x86_64               # Processor architecture
    Environment:
      Variables:
        ENVIRONMENT: production

# Your resources
Resources:
  PitchAnalysisFunction:
    Type: AWS::Serverless::Function  # SAM creates Lambda + permissions
    Properties:
      CodeUri: pitch_analysis/        # Folder with your code
      Handler: app.lambda_handler     # Function to call (app.py → lambda_handler)
      Description: Analyzes pitch from audio file
      Events:                         # What triggers this function?
        PitchAnalysis:
          Type: Api                   # API Gateway trigger
          Properties:
            Path: /analyze             # Endpoint path
            Method: post               # HTTP method

# Outputs (useful info after deployment)
Outputs:
  PitchAnalysisApi:
    Description: "API Gateway endpoint URL"
    Value: !Sub "https://${ServerlessRestApi}.execute-api.${AWS::Region}.amazonaws.com/Prod/analyze/"
  PitchAnalysisFunction:
    Description: "Lambda Function ARN"
    Value: !GetAtt PitchAnalysisFunction.Arn
```

---

### Create Your Lambda Function

**File: `pitch_analysis/app.py`**

```python
"""
Lambda Handler for Pitch Analysis
Entry point for AWS Lambda execution
"""

import json
import base64
import tempfile
import os
import sys

# Lambda has limited packages pre-installed
# We'll add librosa in requirements.txt
# import librosa  # Will add after deploying

def lambda_handler(event, context):
    """
    AWS Lambda entry point
    
    Args:
        event: Contains request data from API Gateway
        {
            "body": "base64_encoded_string",
            "headers": {...},
            "httpMethod": "POST",
            "path": "/analyze"
        }
        
        context: Lambda execution context
        {
            "function_name": "PitchAnalysisFunction",
            "memory_limit_in_mb": 512,
            "request_id": "unique-id",
            "log_group_name": "/aws/lambda/function-name"
        }
    
    Returns:
        {
            "statusCode": 200,
            "headers": {...},
            "body": "json_string"
        }
    """
    
    print(f"🎯 Lambda invoked: {context.function_name}")
    print(f"📝 Request ID: {context.request_id}")
    
    try:
        # Parse request
        if not event.get('body'):
            return error_response(400, "Missing request body")
        
        body = json.loads(event['body'])
        
        # For now, return mock response
        # (We'll add real processing after deployment)
        result = {
            "average_pitch": 220.0,
            "pitch_range": [180, 280],
            "confidence": 0.85,
            "message": "Mock response - replace with real processing"
        }
        
        return success_response(result)
        
    except json.JSONDecodeError:
        return error_response(400, "Invalid JSON in request body")
    except Exception as e:
        print(f"❌ Error: {str(e)}")
        return error_response(500, str(e))

def success_response(data):
    """Helper: Format successful response"""
    return {
        'statusCode': 200,
        'headers': {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*',  # CORS for MAUI
            'Access-Control-Allow-Methods': 'POST, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type'
        },
        'body': json.dumps({
            'success': True,
            'data': data
        })
    }

def error_response(status_code, message):
    """Helper: Format error response"""
    return {
        'statusCode': status_code,
        'headers': {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
        },
        'body': json.dumps({
            'success': False,
            'error': message
        })
    }
```

**File: `pitch_analysis/requirements.txt`**

```txt
# Start minimal, add dependencies as needed
# Note: Lambda has size limits (250MB unzipped)

numpy==1.26.3
# librosa==0.10.1  # Add after testing basic deployment
```

---

### Test Locally (Before Deploying!)

```bash
# Build the Lambda package
sam build

# Should see:
# Building function 'PitchAnalysisFunction'
# Running PythonPipBuilder:ResolveDependencies
# Build Succeeded

# Test locally (simulates Lambda environment)
sam local invoke PitchAnalysisFunction --event events/event.json

# Or start local API
sam local start-api

# Test with curl
curl -X POST http://localhost:3000/analyze \
  -H "Content-Type: application/json" \
  -d '{"audio": "test_data"}'

✅ Local testing works!
```

---

### Deploy to AWS!

```bash
# First deployment (guided)
sam deploy --guided

# Answer prompts:
# Stack Name: myvocalist-pitch-api
# AWS Region: us-east-1 (or sa-east-1)
# Confirm changes before deploy: Y
# Allow SAM CLI IAM role creation: Y
# Disable rollback: N
# PitchAnalysisFunction may not have authorization defined, Is this okay?: Y
# Save arguments to configuration file: Y
# SAM configuration file: samconfig.toml
# SAM configuration environment: default

# Wait 2-3 minutes...
# Deploying with following values
# Stack name                  : myvocalist-pitch-api
# Region                      : us-east-1
# ...
# CloudFormation outputs from deployed stack
# Outputs
# Key                 PitchAnalysisApi
# Value               https://abc123xyz.execute-api.us-east-1.amazonaws.com/Prod/analyze/

✅ DEPLOYED TO AWS!
```

### Test Your Live API

```bash
# Copy the URL from outputs above
export API_URL="https://your-api-id.execute-api.us-east-1.amazonaws.com/Prod/analyze"

# Test with curl
curl -X POST $API_URL \
  -H "Content-Type: application/json" \
  -d '{"audio": "test_base64_data"}'

# Should return:
{
  "success": true,
  "data": {
    "average_pitch": 220.0,
    "pitch_range": [180, 280],
    "confidence": 0.85,
    "message": "Mock response - replace with real processing"
  }
}

✅ YOUR API IS LIVE ON AWS!
```

---

## 📊 Part 4: Monitoring & Logs (15 minutes)

### View Logs in CloudWatch

```bash
# Using AWS CLI
aws logs tail /aws/lambda/myvocalist-pitch-api-PitchAnalysisFunction --follow

# Or using SAM
sam logs -n PitchAnalysisFunction --tail

# You'll see:
# 🎯 Lambda invoked: PitchAnalysisFunction
# 📝 Request ID: abc-123-def-456
# ...
```

### Check Metrics

```bash
# Go to AWS Console:
# CloudWatch > Metrics > Lambda
# 
# You'll see:
# - Invocations (how many times called)
# - Duration (how long it ran)
# - Errors (if any failed)
# - Throttles (if exceeded limits)

# Or via CLI:
aws cloudwatch get-metric-statistics \
  --namespace AWS/Lambda \
  --metric-name Invocations \
  --dimensions Name=FunctionName,Value=myvocalist-pitch-api-PitchAnalysisFunction \
  --start-time $(date -u -d '1 hour ago' +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 3600 \
  --statistics Sum
```

---

## 💰 Part 5: Cost Monitoring Setup (20 minutes)

### Enable Cost Explorer

```bash
# AWS Console > Billing > Cost Explorer
# Click "Enable Cost Explorer"
# Takes ~24 hours to populate

# Set up budget alert:
1. Billing > Budgets > Create budget
2. Use a template: Zero spend budget
3. Budget name: MyVocaList-Free-Tier-Alert
4. Email recipients: your-email@example.com
5. Create budget

✅ You'll get email if you're charged anything!
```

### View Current Usage

```bash
# Check Lambda usage
aws lambda get-function \
  --function-name myvocalist-pitch-api-PitchAnalysisFunction

# Check API Gateway usage
aws apigateway get-usage \
  --usage-plan-id <your-plan-id> \
  --start-date 2024-12-01 \
  --end-date 2024-12-31

# Or use AWS Console:
# Billing Dashboard > Free Tier
# Shows usage vs limits
```

---

## 🔄 Part 6: Update & Redeploy (10 minutes)

### Make Code Changes

```bash
# Edit pitch_analysis/app.py
# Add real pitch detection logic
# (We'll do this in Month 3 of curriculum)

# Rebuild
sam build

# Test locally first!
sam local start-api

# Deploy updates (no --guided needed after first time)
sam deploy

# SAM uses your saved config from samconfig.toml
# Much faster now!

✅ Updates deployed in ~1 minute!
```

---

## 🗑️ Part 7: Cleanup (When Needed)

```bash
# Delete entire stack (removes all AWS resources)
aws cloudformation delete-stack --stack-name myvocalist-pitch-api

# Or using SAM
sam delete --stack-name myvocalist-pitch-api

# Confirm: Yes

# Wait 1-2 minutes...
# Stack deleted!

# This removes:
# - Lambda function
# - API Gateway
# - IAM roles
# - CloudWatch logs (after retention period)

✅ All resources deleted, no charges!
```

---

## 📋 Complete Deployment Checklist

Use this for each new Lambda function:

- [ ] Create project folder
- [ ] `sam init` (or copy existing)
- [ ] Write Lambda handler
- [ ] Add dependencies to requirements.txt
- [ ] `sam build` (local build)
- [ ] `sam local invoke` (local test)
- [ ] `sam deploy --guided` (first deploy)
- [ ] Test live API with curl
- [ ] Check CloudWatch logs
- [ ] Verify free tier usage
- [ ] Document API endpoint in MAUI code

---

## 🎓 Advanced Topics (Month 5-6)

### Lambda Layers (Shared Dependencies)

```yaml
# Create layer for common packages (librosa, numpy)
# Share across multiple functions
# Reduces deployment size

Resources:
  SharedLayer:
    Type: AWS::Serverless::LayerVersion
    Properties:
      LayerName: myvocalist-ml-dependencies
      ContentUri: layers/ml/
      CompatibleRuntimes:
        - python3.11
  
  PitchFunction:
    Type: AWS::Serverless::Function
    Properties:
      Layers:
        - !Ref SharedLayer  # Use shared layer
```

### Environment Variables

```yaml
# Store configuration securely
Globals:
  Function:
    Environment:
      Variables:
        GEMINI_API_KEY: '{{resolve:secretsmanager:gemini-key}}'
        S3_BUCKET: !Ref AudioBucket
        ENVIRONMENT: production
```

### S3 Integration

```yaml
# Add S3 bucket for audio files
Resources:
  AudioBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: myvocalist-audio-files
      LifecycleConfiguration:
        Rules:
          - Id: DeleteOldFiles
            Status: Enabled
            ExpirationInDays: 7  # Auto-delete after 7 days

  PitchFunction:
    Type: AWS::Serverless::Function
    Properties:
      Environment:
        Variables:
          AUDIO_BUCKET: !Ref AudioBucket
      Policies:
        - S3ReadPolicy:
            BucketName: !Ref AudioBucket
```

---

## 🚨 Common Issues & Solutions

### Issue: "Unable to import module 'app'"
```bash
# Cause: Dependencies not installed in Lambda package
# Solution:
# 1. Check requirements.txt exists
# 2. Run: sam build
# 3. Verify: ls .aws-sam/build/FunctionName/
# Dependencies should be there
```

### Issue: Lambda timeout
```bash
# Cause: Function takes >30 seconds
# Solution: Increase timeout in template.yaml
Globals:
  Function:
    Timeout: 60  # Increase to 60 seconds
```

### Issue: Out of memory
```bash
# Cause: Processing large audio files
# Solution: Increase memory
Globals:
  Function:
    MemorySize: 1024  # Increase to 1GB
```

### Issue: Cold start too slow
```bash
# Cause: Loading heavy libraries (librosa)
# Solution:
# 1. Keep function warm (scheduled ping)
# 2. Use Lambda layers
# 3. Optimize imports (lazy loading)

def lambda_handler(event, context):
    # Don't import at top level!
    import librosa  # Import only when needed
    ...
```

---

## 🎯 Next Steps

Now that AWS is set up:

1. **Complete Month 1 curriculum** (Python/ML basics)
2. **Deploy recommendation API** (Month 2)
3. **Add pitch analysis** (Month 3)
4. **Migrate all services to AWS** (Month 5)

---

## 📚 Additional Resources

### AWS Documentation
- [Lambda Developer Guide](https://docs.aws.amazon.com/lambda/)
- [API Gateway Documentation](https://docs.aws.amazon.com/apigateway/)
- [SAM Documentation](https://docs.aws.amazon.com/serverless-application-model/)

### Video Tutorials
- [AWS Lambda for Beginners](https://www.youtube.com/watch?v=eOBq__h4OJ4)
- [SAM CLI Tutorial](https://www.youtube.com/watch?v=MipjLaTp5nA)

### Useful Commands Reference

```bash
# SAM Commands
sam init                    # Create new project
sam build                   # Build Lambda package
sam local invoke           # Test locally
sam local start-api        # Local API server
sam deploy                 # Deploy to AWS
sam logs --tail            # View logs
sam delete                 # Delete stack

# AWS CLI Commands
aws lambda list-functions                    # List all functions
aws lambda invoke --function-name NAME out   # Invoke function
aws logs tail /aws/lambda/NAME --follow      # View logs
aws cloudformation list-stacks               # List all stacks
aws cloudformation describe-stacks           # Stack details

# Debugging
sam local invoke --debug                     # Debug mode
sam validate                                 # Validate template
sam build --use-container                    # Build in Docker
```

---

## ✅ Success Criteria

You're ready to move forward when you can:

- [ ] Deploy Lambda function from scratch
- [ ] Test API with curl/Postman
- [ ] View logs in CloudWatch
- [ ] Understand template.yaml structure
- [ ] Monitor costs in billing dashboard
- [ ] Make code changes and redeploy
- [ ] Delete and recreate stack

**Congratulations! You now have production-grade AWS infrastructure! 🎉**

---

Next guide: `MAUI_INTEGRATION_GUIDE.md` for connecting your C# app to AWS APIs.
