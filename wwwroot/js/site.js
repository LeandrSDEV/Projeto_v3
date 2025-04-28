
document.getElementById('btnProcessar').addEventListener('click', async function () {
    var form = document.querySelector('form');
    var formData = new FormData(form);

    // Desativa o botão para evitar duplo clique
    var btn = document.getElementById('btnProcessar');
    btn.disabled = true;
    btn.innerText = "Processando...";

    try {
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            // Quando o processamento for concluído, abre o modal
            var modalFluxo = new bootstrap.Modal(document.getElementById('modalFluxo'));
            modalFluxo.show();
        } else {
            alert("Erro ao processar arquivo.");
        }
    } catch (error) {
        console.error(error);
        alert("Erro de conexão com o servidor.");
    } finally {
        btn.disabled = false;
        btn.innerText = "Processar";
    }
});

document.getElementById('btnPrefeitura').addEventListener('click', function () {
    window.location.href = '/Fluxo/Prefeitura'; // ou onde quiser redirecionar
});

document.getElementById('btnCargo').addEventListener('click', function () {
    window.location.href = '/Fluxo/Cargo'; // ou outro fluxo
});

