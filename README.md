# LugiaTrack API

API RESTful desenvolvida em ASP.NET Core (.NET 9) com Entity Framework Core, utilizando migrations para gerenciamento de banco de dados Oracle. A API gerencia o cadastro de funcionários e motos, com autenticação via e-mail e senha.

---

## 📌 Descrição

A LugiaTrack API oferece endpoints para:

* 🔐 Login de funcionários
* 👤 Cadastro e gerenciamento de funcionários
* 🏍️ Cadastro e gerenciamento de motos
* 📄 Documentação automática com Swagger (OpenAPI)
* 📥 Endpoint raiz com mensagem com o nome da api

---

## 🚀 Como rodar o projeto

1. Clone o repositório:

   ```bash
   git clone https://github.com/Lugia-Code/.net-api.git
   cd .net-api/lugiatrack-api
   ```

2. Restaure os pacotes e compile:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Rode a aplicação:

   ```bash
   dotnet run
   ```

4. Acesse o Swagger para testar os endpoints:

   ```
   http://localhost:5123/swagger
   ```

---

## 📦 Endpoints

### 🌐 Raiz

| Método | Rota | Descrição                           |
| ------ | ---- | ----------------------------------- |
| GET    | `/`  | Retorna uma mensagem com o nome da api |

---

### 🔐 Login

| Método | Rota     | Descrição                                |
| ------ | -------- | ---------------------------------------- |
| POST   | `/login` | Autentica funcionário por e-mail e senha |

---

### 👤 Funcionários

| Método | Rota            | Descrição                      |
| ------ | --------------- | ------------------------------ |
| GET    | `/funcionarios` | Lista paginada de funcionários |
| POST   | `/funcionarios` | Cria um novo funcionário       |

---

### 🏍️ Motos

| Método | Rota                      | Descrição                   |
| ------ | ------------------------- | --------------------------- |
| GET    | `/motos`                  | Lista paginada de motos     |
| GET    | `/motos/{chassi}/{placa}` | Retorna uma moto específica |
| POST   | `/motos`                  | Cadastra uma nova moto      |
| PUT    | `/motos/{chassi}/{placa}` | Atualiza uma moto existente |
| DELETE | `/motos/{chassi}/{placa}` | Remove uma moto             |

---

## 📁 Estrutura do Projeto

```plaintext
lugiatrack-api/
├── Dtos/
├── Models/
├── Validators/
├── Migrations/
├── Program.cs
├── OracleDbContext.cs
└── README.md
```
