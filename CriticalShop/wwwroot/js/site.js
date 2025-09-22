document.addEventListener('DOMContentLoaded', function() {
    // Inicializar funcionalidades do carrinho se o script carrinho.js estiver carregado
    if (typeof adicionarAoCarrinho === 'function') {
        // O carrinho.js já gerencia os eventos dos botões
        console.log('Sistema de carrinho carregado');
    } else {
        // Fallback para funcionalidade básica
        document.querySelectorAll('.add-to-cart').forEach(button => {
            button.addEventListener('click', function() {
                const productId = this.getAttribute('data-id');
                console.log(`Produto ${productId} adicionado ao carrinho`);
                
                // Feedback visual básico
                this.textContent = 'Adicionado!';
                this.style.background = '#27ae60';
                setTimeout(() => {
                    this.textContent = 'Adicionar ao Carrinho';
                    this.style.background = '';
                }, 2000);
            });
        });
    }
});