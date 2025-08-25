document.addEventListener('DOMContentLoaded', function() {
    // Adicionar ao carrinho
    document.querySelectorAll('.add-to-cart').forEach(button => {
        button.addEventListener('click', function() {
            const productId = this.getAttribute('data-id');
            // Aqui você pode adicionar a lógica para adicionar ao carrinho
            console.log(`Produto ${productId} adicionado ao carrinho`);
            
            // Exemplo de feedback
            this.textContent = 'Adicionado!';
            setTimeout(() => {
                this.textContent = 'Adicionar ao Carrinho';
            }, 2000);
        });
    });
});