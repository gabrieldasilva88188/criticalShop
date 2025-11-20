// ==============================================
// CARRINHO DE COMPRAS - JAVASCRIPT
// ==============================================

document.addEventListener('DOMContentLoaded', function() {
    // Inicializar funcionalidades do carrinho
    inicializarCarrinho();
    
    // Atualizar contador do carrinho no header
    atualizarContadorCarrinho();
});

// ==============================================
// FUNÇÕES PRINCIPAIS
// ==============================================

function inicializarCarrinho() {
    // Configurar eventos para botões de adicionar ao carrinho
    document.querySelectorAll('.add-to-cart').forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const productId = this.getAttribute('data-id');
            adicionarAoCarrinho(productId);
        });
    });
}

// ==============================================
// ADICIONAR AO CARRINHO
// ==============================================

function adicionarAoCarrinho(produtoId, quantidade = 1) {
    const button = document.querySelector(`[data-id="${produtoId}"]`);
    
    if (!button) {
        console.error('Botão não encontrado para produto ID:', produtoId);
        return;
    }
    
    const originalText = button.textContent;
    
    // Feedback visual imediato
    button.textContent = 'Adicionando...';
    button.disabled = true;
    
    console.log('Adicionando produto ao carrinho:', produtoId);
    
    fetch('/Carrinho/Adicionar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            produtoId: parseInt(produtoId),
            quantidade: quantidade
        })
    })
    .then(response => {
        console.log('Resposta recebida:', response.status);
        return response.json();
    })
    .then(data => {
        console.log('Dados recebidos:', data);
        if (data.success) {
            // Feedback de sucesso
            button.textContent = 'Adicionado!';
            button.style.background = '#27ae60';
            
            // Mostrar notificação
            mostrarNotificacao(data.message || 'Produto adicionado ao carrinho!', 'success');
            
            // Atualizar contador do carrinho
            atualizarContadorCarrinho();
            
            // Restaurar botão após 2 segundos
            setTimeout(() => {
                button.textContent = originalText;
                button.style.background = '';
                button.disabled = false;
            }, 2000);
        } else {
            throw new Error(data.message);
        }
    })
    .catch(error => {
        console.error('Erro:', error);
        button.textContent = 'Erro!';
        button.style.background = '#e74c3c';
        
        mostrarNotificacao('Erro ao adicionar produto ao carrinho', 'error');
        
        setTimeout(() => {
            button.textContent = originalText;
            button.style.background = '';
            button.disabled = false;
        }, 2000);
    });
}

// ==============================================
// ATUALIZAR QUANTIDADE
// ==============================================

function atualizarQuantidade(itemId, novaQuantidade) {
    const quantityDisplay = document.getElementById(`quantity-${itemId}`);
    const originalValue = quantityDisplay.textContent;
    
    // Feedback visual
    quantityDisplay.textContent = '...';
    
    fetch('/Carrinho/AtualizarQuantidade', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            itemId: itemId,
            novaQuantidade: novaQuantidade
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Atualizar interface
            quantityDisplay.textContent = novaQuantidade;
            
            // Atualizar totais
            if (data.subtotal) {
                const itemRow = quantityDisplay.closest('.cart-item');
                const subtotalElement = itemRow.querySelector('.item-subtotal strong');
                if (subtotalElement) {
                    subtotalElement.textContent = data.subtotal;
                }
            }
            
            if (data.total) {
                document.getElementById('total-final').innerHTML = `<strong>${data.total}</strong>`;
            }
            
            if (data.totalItens) {
                document.getElementById('total-itens').textContent = data.totalItens;
            }
            
            // Atualizar contador do header
            atualizarContadorCarrinho();
            
        } else {
            throw new Error(data.message);
        }
    })
    .catch(error => {
        console.error('Erro:', error);
        quantityDisplay.textContent = originalValue;
        mostrarNotificacao('Erro ao atualizar quantidade', 'error');
    });
}

// ==============================================
// ALTERAR QUANTIDADE (INCREMENTO/DECREMENTO SEGURO)
// ==============================================

function alterarQuantidade(itemId, delta) {
    const quantityDisplay = document.getElementById(`quantity-${itemId}`);
    if (!quantityDisplay) return;
    const atual = parseInt(quantityDisplay.textContent || '1', 10);
    const nova = Math.max(1, atual + delta);
    if (nova === atual) return; // não envia se não mudou
    atualizarQuantidade(itemId, nova);
}

// ==============================================
// REMOVER ITEM
// ==============================================

function removerItem(itemId) {
    if (!confirm('Tem certeza que deseja remover este item do carrinho?')) {
        return;
    }
    
    const itemElement = document.querySelector(`[data-item-id="${itemId}"]`);
    
    fetch('/Carrinho/RemoverItem', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            itemId: itemId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Remover item da interface com animação
            itemElement.style.opacity = '0';
            itemElement.style.transform = 'translateX(-100%)';
            
            setTimeout(() => {
                itemElement.remove();
                
                // Verificar se carrinho está vazio
                const remainingItems = document.querySelectorAll('.cart-item');
                if (remainingItems.length === 0) {
                    location.reload(); // Recarregar para mostrar carrinho vazio
                }
            }, 300);
            
            // Atualizar totais
            if (data.total) {
                document.getElementById('total-final').innerHTML = `<strong>${data.total}</strong>`;
            }
            
            if (data.totalItens) {
                document.getElementById('total-itens').textContent = data.totalItens;
            }
            
            // Atualizar contador do header
            atualizarContadorCarrinho();
            
            mostrarNotificacao(data.message, 'success');
            
        } else {
            throw new Error(data.message);
        }
    })
    .catch(error => {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao remover item', 'error');
    });
}

// ==============================================
// LIMPAR CARRINHO
// ==============================================

function limparCarrinho() {
    if (!confirm('Tem certeza que deseja limpar todo o carrinho?')) {
        return;
    }
    
    fetch('/Carrinho/Limpar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            mostrarNotificacao(data.message, 'success');
            
            // Recarregar página para mostrar carrinho vazio
            setTimeout(() => {
                location.reload();
            }, 1000);
            
        } else {
            throw new Error(data.message);
        }
    })
    .catch(error => {
        console.error('Erro:', error);
        mostrarNotificacao('Erro ao limpar carrinho', 'error');
    });
}

// ==============================================
// ATUALIZAR CONTADOR DO CARRINHO
// ==============================================

function atualizarContadorCarrinho() {
    fetch('/Carrinho/Contador')
        .then(response => response.json())
        .then(data => {
            const cartCount = document.querySelector('.cart-count');
            if (cartCount) {
                cartCount.textContent = data.totalItens || 0;
            }
        })
        .catch(error => {
            console.error('Erro ao atualizar contador:', error);
        });
}

// ==============================================
// CÁLCULO DE FRETE
// ==============================================

let freteSelecionado = null;

function calcularFrete() {
    const cepInput = document.getElementById('cep-input');
    const cep = cepInput.value.replace(/\D/g, '');
    
    if (cep.length !== 8) {
        mostrarErroFrete('Por favor, digite um CEP válido com 8 dígitos');
        return;
    }
    
    // Mostrar loading
    const btnCalcular = document.querySelector('.btn-calcular-frete');
    const originalText = btnCalcular.innerHTML;
    btnCalcular.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Calculando...';
    btnCalcular.disabled = true;
    
    // Limpar resultados anteriores
    document.getElementById('frete-resultado').style.display = 'none';
    document.getElementById('frete-erro').style.display = 'none';
    
    fetch('/Carrinho/CalcularFrete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            cepDestino: cep
        })
    })
    .then(response => response.json())
    .then(data => {
        btnCalcular.innerHTML = originalText;
        btnCalcular.disabled = false;
        
        if (data.sucesso) {
            mostrarResultadoFrete(data);
        } else {
            mostrarErroFrete(data.mensagem);
        }
    })
    .catch(error => {
        console.error('Erro:', error);
        btnCalcular.innerHTML = originalText;
        btnCalcular.disabled = false;
        mostrarErroFrete('Erro ao calcular frete. Tente novamente.');
    });
}

function mostrarResultadoFrete(data) {
    const freteResultado = document.getElementById('frete-resultado');
    const freteEndereco = document.getElementById('frete-endereco');
    const freteOpcoes = document.getElementById('frete-opcoes');
    
    // Mostrar endereço
    freteEndereco.innerHTML = `
        <p><i class="fas fa-map-marker-alt"></i> ${data.cidade} - ${data.estado}</p>
    `;
    
    // Mostrar opções de frete
    let opcoesHtml = '<div class="frete-opcoes-list">';
    data.opcoes.forEach(opcao => {
        opcoesHtml += `
            <div class="frete-opcao" onclick="selecionarFrete('${opcao.servico}', ${opcao.valor}, '${opcao.valorFormatado}', '${opcao.nome}', ${opcao.prazoEntrega})">
                <div class="frete-opcao-info">
                    <strong>${opcao.nome}</strong>
                    <p>${opcao.observacao}</p>
                    <small>Entrega em até ${opcao.prazoEntrega} dias úteis</small>
                </div>
                <div class="frete-opcao-valor">
                    <strong>${opcao.valorFormatado}</strong>
                </div>
            </div>
        `;
    });
    opcoesHtml += '</div>';
    
    freteOpcoes.innerHTML = opcoesHtml;
    freteResultado.style.display = 'block';
}

function mostrarErroFrete(mensagem) {
    const freteErro = document.getElementById('frete-erro');
    freteErro.textContent = mensagem;
    freteErro.style.display = 'block';
}

function selecionarFrete(servico, valor, valorFormatado, nome, prazo) {
    freteSelecionado = { servico, valor, valorFormatado, nome, prazo };
    
    // Destacar opção selecionada
    document.querySelectorAll('.frete-opcao').forEach(opcao => {
        opcao.classList.remove('selecionado');
    });
    event.currentTarget.classList.add('selecionado');
    
    // Atualizar resumo do pedido
    const freteRow = document.getElementById('frete-selecionado-row');
    const freteValor = document.getElementById('frete-valor');
    const subtotalElement = document.getElementById('subtotal');
    const totalFinal = document.getElementById('total-final');
    
    // Mostrar linha do frete
    freteRow.style.display = 'flex';
    freteValor.textContent = valorFormatado;
    
    // Obter o subtotal do carrinho (sem frete)
    let subtotal = 0;
    if (subtotalElement) {
        const subtotalText = subtotalElement.textContent.trim();
        subtotal = parseFloat(subtotalText.replace(/[^\d,]/g, '').replace(',', '.')) || 0;
    }
    
    // Calcular novo total (subtotal + frete)
    const novoTotal = subtotal + valor;
    
    // Atualizar o total com frete
    totalFinal.innerHTML = `<strong>${novoTotal.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</strong>`;
    
    // Armazenar o valor do frete para uso posterior
    document.querySelector('.carrinho-container').dataset.frete = valor;
    
    mostrarNotificacao(`Frete ${nome} selecionado!`, 'success');
    
    // Rolar suavemente até o resumo para mostrar o valor atualizado
    document.querySelector('.cart-summary').scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

// Máscara de CEP
document.addEventListener('DOMContentLoaded', function() {
    const cepInput = document.getElementById('cep-input');
    if (cepInput) {
        cepInput.addEventListener('input', function(e) {
            let value = e.target.value.replace(/\D/g, '');
            if (value.length > 5) {
                value = value.substring(0, 5) + '-' + value.substring(5, 8);
            }
            e.target.value = value;
        });
        
        // Permitir calcular com Enter
        cepInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                calcularFrete();
            }
        });
    }
});

// ==============================================
// SISTEMA DE NOTIFICAÇÕES
// ==============================================

function mostrarNotificacao(mensagem, tipo = 'info') {
    // Remover notificação existente
    const existingNotification = document.querySelector('.notification');
    if (existingNotification) {
        existingNotification.remove();
    }
    
    // Criar nova notificação
    const notification = document.createElement('div');
    notification.className = `notification notification-${tipo}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas ${getIconForType(tipo)}"></i>
            <span>${mensagem}</span>
            <button class="notification-close" onclick="this.parentElement.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;
    
    // Adicionar ao DOM
    document.body.appendChild(notification);
    
    // Mostrar com animação
    setTimeout(() => {
        notification.classList.add('show');
    }, 100);
    
    // Remover automaticamente após 5 segundos
    setTimeout(() => {
        if (notification.parentElement) {
            notification.classList.remove('show');
            setTimeout(() => {
                if (notification.parentElement) {
                    notification.remove();
                }
            }, 300);
        }
    }, 5000);
}

function getIconForType(tipo) {
    switch (tipo) {
        case 'success': return 'fa-check-circle';
        case 'error': return 'fa-exclamation-circle';
        case 'warning': return 'fa-exclamation-triangle';
        default: return 'fa-info-circle';
    }
}

// ==============================================
// ESTILOS PARA NOTIFICAÇÕES (adicionados dinamicamente)
// ==============================================

if (!document.querySelector('#notification-styles')) {
    const style = document.createElement('style');
    style.id = 'notification-styles';
    style.textContent = `
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            background: white;
            border-radius: 10px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
            z-index: 10000;
            max-width: 400px;
            transform: translateX(100%);
            opacity: 0;
            transition: all 0.3s ease;
        }
        
        .notification.show {
            transform: translateX(0);
            opacity: 1;
        }
        
        .notification-content {
            display: flex;
            align-items: center;
            padding: 15px 20px;
            gap: 12px;
        }
        
        .notification-success {
            border-left: 4px solid #27ae60;
        }
        
        .notification-error {
            border-left: 4px solid #e74c3c;
        }
        
        .notification-warning {
            border-left: 4px solid #f39c12;
        }
        
        .notification-info {
            border-left: 4px solid #3498db;
        }
        
        .notification-content i {
            font-size: 1.2rem;
        }
        
        .notification-success .notification-content i {
            color: #27ae60;
        }
        
        .notification-error .notification-content i {
            color: #e74c3c;
        }
        
        .notification-warning .notification-content i {
            color: #f39c12;
        }
        
        .notification-info .notification-content i {
            color: #3498db;
        }
        
        .notification-content span {
            flex: 1;
            font-weight: 500;
            color: #2c3e50;
        }
        
        .notification-close {
            background: none;
            border: none;
            color: #6c757d;
            cursor: pointer;
            padding: 5px;
            border-radius: 50%;
            transition: all 0.3s ease;
        }
        
        .notification-close:hover {
            background: #f8f9fa;
            color: #2c3e50;
        }
    `;
    document.head.appendChild(style);
}

