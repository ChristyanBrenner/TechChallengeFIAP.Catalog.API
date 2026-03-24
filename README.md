# TechChallengeFIAP.Catalog.API
API de Catálogo do serviço reponsável por expor endpoints REST para consulta e gerenciamento de catálogo de jogos.

## Tecnologias Utilizadas
- C# / .NET 8
- ASP.NET Core Web API
- Docker
- Kubernetes
- Amazon SQS

## Instruções
SUBIR IMAGEM DOCKER LOCAL
- docker build --no-cache -t catalog-api . && docker run -d -p 5001:80 catalog-api

SUBIR O REPOSITORIO NO AWS App Runner
- aws ecr get-login-password --region sa-east-1 | docker login --username AWS --password-stdin 451664151831.dkr.ecr.sa-east-1.amazonaws.com
 
- docker tag catalog-api:latest 451664151831.dkr.ecr.sa-east-1.amazonaws.com/catalog-api:latest
 
- docker push 451664151831.dkr.ecr.sa-east-1.amazonaws.com/catalog-api:latest

## Fluxo de Comunicação

<img width="1000" height="1000" alt="mermaid-ai-diagram-2026-03-24-204247" src="https://github.com/user-attachments/assets/5309d5f0-08c3-42de-97b6-ab153a1dbf98" />
