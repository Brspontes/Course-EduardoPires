using AutoMapper;
using Dev.IO.API.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dev.IO.API.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IProdutoService produtoService;
        private readonly IMapper mapper;

        public ProdutosController(INotificador notificador, IProdutoRepository produtoRepository, IProdutoService produtoService, IMapper mapper) 
            : base(notificador)
        {
            this.produtoRepository = produtoRepository;
            this.produtoService = produtoService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ProdutosViewModel>> ObterTodos() =>
            Ok(mapper.Map<ProdutosViewModel>(await produtoRepository.ObterProdutosFornecedores()));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutosViewModel>> ObterPorId([FromRoute] Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();

            return Ok(produtoViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutosViewModel>> Adicionar(ProdutosViewModel produtosViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtosViewModel.Imagem;

            if (!UploadArquivo(produtosViewModel.ImagemUpload, imagemNome))
                return CustomResponse(produtosViewModel);

            produtosViewModel.Imagem = imagemNome;

            await produtoService.Adicionar(mapper.Map<Produto>(produtosViewModel));
            return CustomResponse(produtosViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutosViewModel>> Excluir([FromRoute] Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel == null) return NotFound();

            await produtoService.Remover(id);
            return CustomResponse(produtoViewModel);
        }

        private async Task<ProdutosViewModel> ObterProduto(Guid id) =>
            mapper.Map<ProdutosViewModel>(await produtoRepository.ObterPorId(id));

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            var imageDataByteArray = Convert.FromBase64String(arquivo);

            if(string.IsNullOrEmpty(arquivo))
            {
                //Outra possibilidade
                //ModelState.AddModelError(string.Empty, "Forneça uma imagem para este produto");
                NotificarErro("Forneça uma imagem para este produto");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                //ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome");
                NotificarErro("Já existe um arquivo com este nome");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);
            return true;
        }
    }
}
