using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dev.IO.API.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dev.IO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public abstract class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository fornecedorRepository;
        private readonly IMapper mapper;
        private readonly IFornecedorService fornecedorService;

        protected FornecedoresController(IFornecedorRepository fornecedorRepository, IMapper mapper, IFornecedorService fornecedorService)
        {
            this.fornecedorRepository = fornecedorRepository;
            this.mapper = mapper;
            this.fornecedorService = fornecedorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedor = mapper.Map<IEnumerable<FornecedorViewModel>>
                (await fornecedorRepository.ObterTodos());
            return Ok(fornecedor);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(id);

            if (fornecedor == null)
                return NotFound();

            return Ok(fornecedor);
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var fornecedor = mapper.Map<Fornecedor>(fornecedorViewModel);
            var result = await fornecedorService.Adicionar(fornecedor);

            if (!result)
                return BadRequest();

            return Ok(fornecedor);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar([FromRoute] Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            var fornecedor = mapper.Map<Fornecedor>(fornecedorViewModel);
            var result = await fornecedorService.Atualizar(fornecedor);

            if (!result)
                return BadRequest();

            return Ok(fornecedor);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);
            if (fornecedor == null)
                return NotFound();

            var result = await fornecedorService.Remover(id);
            if (!result) return BadRequest();

            return Ok(fornecedor);
        }

        public async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return mapper.Map<FornecedorViewModel>
                (await fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        public async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return mapper.Map<FornecedorViewModel>
                (await fornecedorRepository.ObterFornecedorEndereco(id));
        }
    }
}