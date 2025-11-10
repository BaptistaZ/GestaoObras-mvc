📘 Gestão de Obras — ASP.NET Core MVC

Aplicação Web para gestão de obras, clientes, materiais e operações associadas.
Desenvolvido em C# ASP.NET Core MVC, com Entity Framework Core e PostgreSQL.

⸻

🚀 Funcionalidades Principais

🔹 Gestão de Clientes
	•	CRUD completo (Nome, NIF, Morada, Email, Telefone)
	•	Pesquisa por texto livre
	•	Ordenação dinâmica
	•	Paginação configurável

🔹 Gestão de Materiais
	•	CRUD completo
	•	Stock disponível em tempo real
	•	Movimentos de entrada/saída com impacto no stock

🔹 Gestão de Obras
	•	Dados gerais (cliente, morada, coordenadas)
	•	Abas para:
	•	Registo de Material
	•	Mão-de-Obra
	•	Pagamentos
	•	Registos organizados e formulários dedicados

🔹 Movimentos de Material
	•	Lançamento rápido
	•	Atualização automática de stock
	•	Histórico completo por obra

🔹 Dashboard
	•	Obras ativas
	•	Total de clientes
	•	Total de materiais
	•	Movimentos do dia
	•	Últimos movimentos registados

🔹 Interface (AdminLTE)
	•	Sidebar dinâmica com item ativo
	•	Layout moderno e responsivo
	•	Alertas automáticos (TempData)

⸻

🛠️ Tecnologias Utilizadas
	•	ASP.NET Core MVC 8
	•	Entity Framework Core
	•	PostgreSQL + Npgsql
	•	Bootstrap 5 + AdminLTE
	•	Font Awesome

⸻

📦 Como Executar o Projeto

1️⃣ Clonar o repositório

git clone 
cd GestaoObras-mvc

2️⃣ Restaurar dependências

dotnet restore

3️⃣ Configurar a connection string
Editar o ficheiro:

GestaoObras.Web/appsettings.json

4️⃣ Aplicar migrações da BD

dotnet ef database update

5️⃣ Executar o projeto

dotnet run –project GestaoObras.Web

⸻

🧩 Estrutura do Projeto

GestaoObras.Domain → Entidades e enums
GestaoObras.Data → DbContext, migrações EF Core
GestaoObras.Web → MVC (Controllers, Views, ViewModels)
wwwroot → AdminLTE, CSS, JS e assets

⸻

📄 Licença

Projeto académico — livre para estudo e evolução.