﻿/*Conexão e reconexão signalr */
var connection = new signalR.HubConnectionBuilder().withUrl("/ZapWebHub").build();
var nomeGrupo = "";
function ConnectionStart() {
    connection.start().then(function () {
        HabilitarCadastro();
        HabilitarLogin();
        HabilitarConversacao();
        console.info("Connected!");
    }).catch(function (err) {
        console.error(err.toString());
        setTimeout(ConnectionStart(), 5000);
    });
}
connection.onclose(async () => { await ConnectionStart(); });

/*  */
function HabilitarCadastro() {

    var form = document.getElementById("form_cadastro");

    if (form !== null) {
        var btnCadastrar = document.getElementById("btnCadastrar");
        btnCadastrar.addEventListener("click", function () {
            var nome = document.getElementById("nome").value;
            var email = document.getElementById("email").value;
            var senha = document.getElementById("senha").value;

            var usuario = { Nome: nome, Email: email, Senha: senha };

            connection.invoke("Cadastrar", usuario);
        });
    }

    connection.on("ReceberCadastro", function (sucesso, usuario, msg) {
        var mensagem = document.getElementById("mensagem");
        if (sucesso) {
            console.info(usuario);
            document.getElementById("nome").value = "";
            document.getElementById("email").value = "";
            document.getElementById("senha").value = "";
        }
        mensagem.innerText = msg;
    });

  
}


function HabilitarLogin() {
    var form = document.getElementById("form_login");
    if (form !== null) {
        if (GetUsuarioLogado() !== null) {
            window.location.href = "/Home/Conversacao";
        }
        var btnAcessar = document.getElementById("btnAcessar");
        btnAcessar.addEventListener("click", function () {

            var email = document.getElementById("email").value;
            var senha = document.getElementById("senha").value;

            var usuario = { Email: email, Senha: senha };

            connection.invoke("Login", usuario);

        });
    }
        connection.on("ReceberLogin", function (sucesso, usuario, msg) {

            if (sucesso) {
                SetUsuarioLogado(usuario);
                window.location.href = "/Home/Conversacao";
            } else {
                var mensagem = document.getElementById("mensagem");
                mensagem.innerText = msg;
            }

        });
}
var telaConversacao = document.getElementById("tela-conversacao");
if (telaConversacao !== null) {

    if (GetUsuarioLogado() === null) {
        window.location.href = "/Home/Login";
    }
}

function HabilitarConversacao() {
    MonitorarConnectionId();
    MonitorarListaUsuarios();
    EnviarReceberMensagem();
    AbrirGrupo();
}
function AbrirGrupo() {
    connection.on("AbrirGrupo", function (nomeGrupo) {

        nomeGrupo = nomeGrupo;

    });
}
function EnviarReceberMessagem() {
    var btnEnviar = document.getElementById("btnEnviar");
    btnEnviar.addEventListener("click", function () {
    var mensagem = document.getElementById("mensagem");
        var usuario = GetUsuarioLogado();
        connection.invoke("EnviarMensagem",usuarios, mensagem, nomeGrupo);
    });
}
function MonitorarListaUsuarios() {
    connection.invoke("ObterListUsuarios");
    connection.on("ReceberListUsuarios", function (usuarios) {
        var html = "";
        for (var i = 0; i < usuarios.length; i++) {

            if (usuarios[i].id !== GetUsuarioLogado().id) {
                html += '<div class="container-user-item"><img src = "/imagem/logo.png" style = "width: 20%;" /><div><span>' + usuarios[i].nome + ' (' + (usuarios[i].isOnline ? "online" : "offline") + ')</span><br /><span class="email">' + usuarios[i].email + '</span></div></div>';
            }
        }
        document.getElementById("users").innerHTML = html;
        var container_usuarios = document.getElementById("users").querySelectorAll("container-user-item");
        for (var i = 0; i < container_usuarios.length; i++) {

            container_usuarios[i].addEventListener("click", function (event) {
                var componente = event.target || event.srcElement;
                var emailUsuarioUm = GetUsuarioLogado().email;
                var emailUsuarioDois = componente.parentElement.querySelector(".email").innerText;

                connection.invoke("CriarOuAbrirGrupo", emailUsuarioUm, emailUsuarioDois);

            });
        }
    });
}
function MonitorarConnectionId() {
    var telaConversacao = document.getElementById("tela-conversacao");
    if (telaConversacao !== null) {

        connection.invoke("AddConnectionIdDoUsuario", GetUsuarioLogado());

        var btnSair = document.getElementById("btnSair");
        btnSair.addEventListener("click", function () {
            connection.invoke("Logout", GetUsuarioLogado()).then(() => function () {
                DelUsuarioLogado();
                window.location.href = "/Home/Login";
            });

        });
    }
}


function GetUsuarioLogado() {
    return JSON.parse(sessionStorage.getItem("Logado"));
}
function SetUsuarioLogado(usuario) {
    sessionStorage.setItem("Logado", JSON.stringify(usuario));
}
function DelUsuarioLogado() {
    sessionStorage.removeItem("Logado");
}
ConnectionStart();