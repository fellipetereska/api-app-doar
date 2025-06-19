# 👐 Plataforma DOAR

**Sistema de gerenciamento de doacoes fisicas entre pessoas e instituicoes.**

<div align="center">

📍 Desenvolvido por:

* Felipe Ferreira — RA: 23176241-2
* Fellipe Tereska — RA: 23233031-2

📍 Londrina – 2025

</div>

---

## 📖 Sobre o Projeto

O projeto **DOAR** nasceu da observacao de dificuldades enfrentadas por instituicoes filantropicas, como a **Caritas Arquidiocesana de Londrina**, no gerenciamento de doacoes fisicas. Desenvolvemos uma plataforma moderna, intuitiva e eficiente que conecta doadores a ONGs, promovendo uma gestao organizada, transparente e eficaz dos processos de doacao.

---

## 🎯 Objetivos

* Conectar doadores a instituicoes proximas via geolocalizacao;
* Facilitar o cadastro e a triagem de doacoes;
* Organizar o controle de estoque e assistidos pelas instituicoes;
* Gerar documentos (PDF) que formalizem a doacao;
* Automatizar comunicacoes e oferecer assistencia virtual inteligente.

---

## 👥 Publico-Alvo

* **Doadores** (pessoa fisica ou juridica): interessados em doar roupas, moveis, utensilios e outros itens nao pereciveis.
* **Instituicoes (ONGs)**: organizacoes responsaveis por aceitar, controlar e repassar doacoes aos assistidos.

---

## 🚀 Funcionalidades Principais

* Cadastro de doadores e instituicoes;
* Mapa com geolocalizacao de instituicoes proximas (Leaflet);
* Cadastro completo de doacoes com imagens, categorias e quantidade;
* Gestao de estoque pelas instituicoes;
* Lista de espera de assistidos;
* Emissao de termo de doacao em PDF;
* Roteamento via Google Maps ate a residencia dos assistidos;
* Mensagens automatizadas via WhatsApp ao aceitar doacoes;
* Assistente virtual com IA Gemini para suporte ao doador na pagina inicial.

---

## ✨ Diferenciais

* Mediacao digital direta entre doador e ONG;
* Processo automatizado de documentos e comunicacoes;
* Interface amigavel com foco em usabilidade;
* Baseado em problema real observado na Caritas;
* IA integrada (Gemini) para assistencia inteligente ao usuario.

---

## 🛠 Tecnologias Utilizadas

**Front-end:**

* React.js
* Tailwind CSS

**Back-end:**

* ASP.NET Core (C#)
* Dapper / Dapper.Contrib

**Banco de Dados:**

* MySQL

**Outras Bibliotecas:**

* Leaflet
* Google Maps API
* iTextSharp (PDF)
* JWT
* Twilio API / Webhooks WhatsApp
* Gemini (IA assistente virtual)

---

## 📂 Estrutura de Diretórios

### Back-end (`/api-app-doar`)

```
├── Automation/         # Integracao com WhatsApp
├── Controllers/        # Endpoints da API
├── Classes/            # Entidades do sistema
├── Repositories/       # Acesso a dados com Dapper
├── DTOs/               # Objetos de Transferencia de Dados
├── Utils/              # Validacoes e helpers
├── PersistenciaDB/     # Conexao com banco
├── Services/           # Regras de negocio
├── Uploads/            # Armazenamento de imagens
├── Reports/            # Templates de relatorios PDF
```

### Front-end (`/app-doar`)

```
├── components/         # Componentes reutilizaveis
├── pages/              # Paginas por rota
├── services/           # Integracao com API
├── contexts/           # Contextos globais
├── hooks/              # Hooks personalizados
├── media/              # Imagens e arquivos estaticos
```

---

## ✅ Funcionalidades Implementadas

* Autenticacao com senha criptografada;
* Cadastro de doacoes com fotos e categorias;
* Controle de estoque automatizado;
* Gestao de assistidos e lista de espera;
* Aceitacao/rejeicao de doacoes pela instituicao;
* Geracao de termo de doacao em PDF;
* Rota para entrega com Google Maps;
* Mensagem automatica via WhatsApp apos aceite;
* Assistente virtual IA para tirar duvidas na home.

---

## 💾 Como Instalar e Rodar Localmente

### Back-end

```bash
git clone https://github.com/fellipetereska/api-app-doar
cd api.AppDoar
dotnet run
```

### Front-end

```bash
git clone https://github.com/fellipetereska/app-doar
cd app-doar
npm install
npm start
```

---

## 👨‍💻 Manual do Usuario

### Fluxo do Doador

1. Cria conta e acessa sistema;
2. Encontra ONG via mapa interativo;
3. Cadastra doacao com fotos e categorias;
4. ONG avalia (aceita ou recusa);
5. Recebe notificacao via WhatsApp ao ser aceita;
6. Visualiza status e termo PDF na plataforma.

### Telas Disponiveis

* Tela de login/cadastro
* Mapa com instituicoes proximas
* Painel de doador (historico)
* Painel da ONG (assistidos, estoque, relatorios)
* Visualizacao do termo PDF

---

## 📌 Conclusao

A aplicacao DOAR atingiu os objetivos esperados, oferecendo uma plataforma funcional, organizada e moderna para a gestao de doacoes. O envolvimento direto com a Caritas proporcionou uma solucao alinhada com necessidades reais. A integracao de tecnologias como IA e automacoes garantem praticidade e escalabilidade para o futuro.

---

## 📚 Referencias

SOUZA, Diego M. A. de et al. *Elaboracao de relatorio tecnico-cientifico*. Sao Carlos: ICMC/USP, 2006.
Disponivel em: [https://web.icmc.usp.br/SCATUSU/RT/Relatorios\_Tecnicos/BIBLIOTECA\_113\_RT\_279.pdf](https://web.icmc.usp.br/SCATUSU/RT/Relatorios_Tecnicos/BIBLIOTECA_113_RT_279.pdf)
