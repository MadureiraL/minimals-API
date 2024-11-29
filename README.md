# Documentação da API

## Visão Geral
Esta API fornece endpoints para gerenciar administradores, veículos e a página inicial. Abaixo estão os detalhes dos endpoints disponíveis.

## Informações Gerais
O gerenciamento de alguns protocolos só pode ser feito pelos administradores com a role **Adm**. Abaixo estão as informações do administrador geral que gerencia toda a aplicação:

```json
{
  "Id": ,
  "Email": "administrador@teste.com",
  "Senha": "123456",
  "Perfil": "Adm"
}

## Endpoints

### Home

#### GET /

Retorna a página inicial.

 **URL:** /
    
-   **Método:** GET
    
-   **Autenticação:** Não
    
-   **Tags:** Home
    

### Administradores

#### POST /administradores/login

Valida o login e a senha de um administrador e retorna um token JWT se as credenciais forem válidas.

-   **URL:** /administradores/login
    
-   **Método:** POST
    
-   **Autenticação:** Não
    
-   **Tags:** Administradores
    
-   **Parâmetros:**
    
    -   `loginDTO` (corpo da requisição): Objeto contendo as credenciais de login.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 200 OK
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Email": "string",
          "Perfil": "string",
          "Token": "string"
        }
        
        ```
        
-   **Resposta de Erro:**
    
    -   **Código:** 401 Unauthorized
        

#### GET /administradores

Retorna todos os administradores.

-   **URL:** /administradores
    
-   **Método:** GET
    
-   **Autenticação:** Sim
    
-   **Tags:** Administradores
    
-   **Parâmetros:**
    
    -   `pagina` (query): Número da página para paginação.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 200 OK
        
    -   **Corpo:**
        
        json
        
        ```
        [
          {
            "Id": "int",
            "Email": "string",
            "Perfil": "string"
          }
        ]
        
        ```
        

#### GET /administradores{id}/

Retorna um administrador específico pelo ID.

-   **URL:** /administradores{id}/
    
-   **Método:** GET
    
-   **Autenticação:** Sim
    
-   **Tags:** Administradores
    
-   **Parâmetros:**
    
    -   `id` (rota): ID do administrador.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 200 OK
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Id": "int",
          "Email": "string",
          "Perfil": "string"
        }
        
        ```
        
-   **Resposta de Erro:**
    
    -   **Código:** 404 Not Found
        

#### POST /administradores

Cria um novo administrador e retorna erros de validação caso algum campo esteja vazio.

-   **URL:** /administradores
    
-   **Método:** POST
    
-   **Autenticação:** Sim
    
-   **Tags:** Administradores
    
-   **Parâmetros:**
    
    -   `administradorDTO` (corpo da requisição): Objeto contendo os dados do administrador.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 201 Created
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Id": "int",
          "Email": "string",
          "Perfil": "string"
        }
        
        ```
        
-   **Resposta de Erro:**
    
    -   **Código:** 400 Bad Request
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Mensagens": ["string"]
        }
        
        ```
        

### Veículos

#### POST /veiculos/

Cria um novo veículo e retorna erros de validação caso algum campo esteja vazio.

-   **URL:** /veiculos/
    
-   **Método:** POST
    
-   **Autenticação:** Sim
    
-   **Tags:** Veiculos
    
-   **Parâmetros:**
    
    -   `veiculoDTO` (corpo da requisição): Objeto contendo os dados do veículo.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 201 Created
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Id": "int",
          "Nome": "string",
          "Marca": "string",
          "Ano": "int"
        }
        
        ```
        
-   **Resposta de Erro:**
    
    -   **Código:** 400 Bad Request
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Mensagens": ["string"]
        }
        
        ```
        

#### GET /veiculos/

Retorna todos os veículos registrados no banco.

-   **URL:** /veiculos/
    
-   **Método:** GET
    
-   **Autenticação:** Sim
    
-   **Tags:** Veiculos
    
-   **Parâmetros:**
    
    -   `pagina` (query): Número da página para paginação.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 200 OK
        
    -   **Corpo:**
        
        json
        
        ```
        [
          {
            "Id": "int",
            "Nome": "string",
            "Marca": "string",
            "Ano": "int"
          }
        ]
        
        ```
        

#### GET /veiculos{id}/

Retorna um veículo específico pelo ID.

-   **URL:** /veiculos{id}/
    
-   **Método:** GET
    
-   **Autenticação:** Sim
    
-   **Tags:** Veiculos
    
-   **Parâmetros:**
    
    -   `id` (rota): ID do veículo.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 200 OK
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Id": "int",
          "Nome": "string",
          "Marca": "string",
          "Ano": "int"
        }
        
        ```
        
-   **Resposta de Erro:**
    
    -   **Código:** 404 Not Found
        

#### PUT /veiculos{id}/

Atualiza um veículo existente.

-   **URL:** /veiculos{id}/
    
-   **Método:** PUT
    
-   **Autenticação:** Sim
    
-   **Tags:** Veiculos
    
-   **Parâmetros:**
    
    -   `id` (rota): ID do veículo.
        
    -   `veiculoDTO` (corpo da requisição): Objeto contendo os dados atualizados do veículo.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 200 OK
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Id": "int",
          "Nome": "string",
          "Marca": "string",
          "Ano": "int"
        }
        
        ```
        
-   **Resposta de Erro:**
    
    -   **Código:** 400 Bad Request
        
    -   **Corpo:**
        
        json
        
        ```
        {
          "Mensagens": ["string"]
        }
        
        ```
        
    -   **Código:** 404 Not Found
        

#### DELETE /veiculos{id}/

Deleta um veículo existente.

-   **URL:** /veiculos{id}/
    
-   **Método:** DELETE
    
-   **Autenticação:** Sim
    
-   **Tags:** Veiculos
    
-   **Parâmetros:**
    
    -   `id` (rota): ID do veículo.
        
-   **Resposta de Sucesso:**
    
    -   **Código:** 204 No Content
        
-   **Resposta de Erro:**
    
    -   **Código:** 404 Not Found
