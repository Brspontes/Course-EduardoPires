using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dev.IO.API.ViewModels;
using DevIO.Business.Intefaces;
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

        protected FornecedoresController(IFornecedorRepository fornecedorRepository, IMapper mapper)
        {
            this.fornecedorRepository = fornecedorRepository;
            this.mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedor = mapper.Map<IEnumerable<FornecedorViewModel>>
                (await fornecedorRepository.ObterTodos());
            return Ok(fornecedor);
        }
    }
}