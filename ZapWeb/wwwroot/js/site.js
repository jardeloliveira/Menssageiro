/*Conexão e reconexão signalr */
var connection = new signalR.HubConnectionBuilder().withUrl("/ZapWebHub").build();

function ConnectionStart() {
    connection.start().then(function () {
        HabilitarCadastro();
        HabilitarLogin();
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

  

    function GetUsuarioLogado() {
        return JSON.parse(sessionStorage.getItem("Logado"));
    }
    function SetUsuarioLogado(usuario) {
        sessionStorage.setItem("Logado", JSON.stringify(usuario));
    }


ConnectionStart();