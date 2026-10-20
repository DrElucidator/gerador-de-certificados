import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { form, FormField, required } from '@angular/forms/signals';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { ApiValidationProblemDetails } from '../../auth/auth.models';
import { CursoDto } from '../cursos.models';
import { CursosService } from '../cursos.service';
import {
  CertificadoDto,
  SolicitarCertificadosRequest,
  StatusProcessamentoDto,
} from './certificados.models';
import { CertificadosService } from './certificados.service';

interface CertificadosFormModel {
  readonly nomesAlunos: string;
}

@Component({
  imports: [FormField, RouterLink],
  selector: 'app-certificados-curso',
  templateUrl: './certificados-curso.html',
})
export class CertificadosCurso {
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly cursosService = inject(CursosService);
  private readonly certificadosService = inject(CertificadosService);
  private atualizacaoStatusAgendada?: ReturnType<typeof setTimeout>;

  protected readonly cursoId = this.route.snapshot.paramMap.get('cursoId') ?? '';
  protected readonly curso = signal<CursoDto | null>(null);
  protected readonly mensagemSucesso = signal('');
  protected readonly mensagemErro = signal('');
  protected readonly carregando = signal(true);
  protected readonly enviando = signal(false);
  protected readonly modelo = signal<CertificadosFormModel>({ nomesAlunos: '' });
  protected readonly formulario = form(this.modelo, (modelo) => {
    required(modelo.nomesAlunos, { message: 'Informe ao menos um nome de aluno.' });
  });
  protected readonly status = signal<StatusProcessamentoDto | null>(null);
  protected readonly certificados = signal<readonly CertificadoDto[]>([]);

  constructor() {
    this.destroyRef.onDestroy(() => this.cancelarAtualizacaoStatus());

    this.cursosService.obterPorId(this.cursoId).subscribe({
      next: (curso) => {
        this.curso.set(curso);
        this.carregando.set(false);
        this.carregarStatusProcessamento();
      },
      error: (resposta: HttpErrorResponse) => {
        this.mensagemErro.set(
          this.obterMensagemErroApi(resposta, 'Não foi possível carregar os dados do curso.'),
        );
        this.carregando.set(false);
      },
    });
  }

  protected solicitarGeracao(event: SubmitEvent): void {
    event.preventDefault();

    const nomes = this.modelo()
      .nomesAlunos.split(/\r?\n/)
      .map((nome) => nome.trim())
      .filter(Boolean);

    if (nomes.length === 0) {
      this.formulario.nomesAlunos().markAsTouched();
      return;
    }

    const indiceNomeGrande = nomes.findIndex((nome) => nome.length > 200);

    if (indiceNomeGrande !== -1) {
      this.mensagemErro.set(
        `O nome do aluno na linha ${indiceNomeGrande + 1} excede o limite de 200 caracteres.`,
      );
      return;
    }

    this.enviando.set(true);
    this.mensagemErro.set('');
    this.mensagemSucesso.set('');

    const requisicao: SolicitarCertificadosRequest = { alunos: nomes };

    this.certificadosService.solicitar(this.cursoId, requisicao).subscribe({
      next: () => {
        this.enviando.set(false);
        this.formulario.nomesAlunos().reset('');
        this.mensagemSucesso.set('Solicitação de geração recebida.');
        this.carregarStatusProcessamento();
      },
      error: (resposta: HttpErrorResponse) => {
        this.enviando.set(false);
        this.mensagemErro.set(
          this.obterMensagemErroApi(
            resposta,
            'Não foi possível solicitar a geração dos certificados do curso.',
          ),
        );
      },
    });
  }

  protected atualizarStatus(): void {
    this.cancelarAtualizacaoStatus();
    this.carregarStatusProcessamento();
  }

  private carregarStatusProcessamento(): void {
    this.certificadosService.obterStatus(this.cursoId).subscribe({
      next: (status) => {
        this.status.set(status);
        this.carregarCertificados();

        if (this.processamentoEmAndamento(status.status)) {
          this.agendarAtualizacaoStatus();
        }
      },
      error: (resposta: HttpErrorResponse) => {
        this.status.set(null);

        if (resposta.status !== 404) {
          this.mensagemErro.set(
            this.obterMensagemErroApi(
              resposta,
              'Não foi possível consultar o processamento dos certificados do curso.',
            ),
          );
        }

        this.carregarCertificados();
      },
    });
  }

  private carregarCertificados(): void {
    this.certificadosService.listar(this.cursoId).subscribe({
      next: (certificados) => this.certificados.set(certificados),
      error: (resposta: HttpErrorResponse) => {
        if (resposta.status !== 404) {
          this.mensagemErro.set(
            this.obterMensagemErroApi(
              resposta,
              'Não foi possível listar os certificados do curso.',
            ),
          );
        }
      },
    });
  }

  private processamentoEmAndamento(status: string): boolean {
    return status === 'Pendente' || status === 'GerandoCertificados' || status === 'GerandoZip';
  }

  private agendarAtualizacaoStatus(): void {
    this.cancelarAtualizacaoStatus();

    this.atualizacaoStatusAgendada = setTimeout(() => {
      this.carregarStatusProcessamento();
    }, 2_000);
  }

  private cancelarAtualizacaoStatus(): void {
    if (this.atualizacaoStatusAgendada) {
      clearTimeout(this.atualizacaoStatusAgendada);
      this.atualizacaoStatusAgendada = undefined;
    }
  }

  private obterMensagemErroApi(resposta: HttpErrorResponse, mensagemPadrao: string): string {
    const problem = resposta.error as ApiValidationProblemDetails | null;

    if (problem?.errors) {
      const mensagens = Object.values(problem.errors).flat();

      if (mensagens.length > 0) return mensagens.join('\n');
    }

    return problem?.detail ?? mensagemPadrao;
  }
}
