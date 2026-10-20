export interface SolicitarCertificadosRequest {
  readonly alunos: readonly string[];
}

export interface SolicitacaoCertificadoDto {
  readonly solicitacaoId: string;
  readonly cursoId: string;
  readonly status: string;
  readonly dataSolicitacao: string;
}

export interface StatusProcessamentoDto {
  readonly solicitacaoId: string;
  readonly cursoId: string;
  readonly status: string;
  readonly dataSolicitacao: string;
  readonly quantidadeTotal: number;
  readonly quantidadeGerada: number;
  readonly quantidadeFalha: number;
}

export interface CertificadoDto {
  readonly id: string;
  readonly nomeAluno: string;
  readonly status: string;
  readonly dataGeracao: string | null;
}
