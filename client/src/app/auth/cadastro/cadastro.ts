import { afterNextRender, Component, ElementRef, signal, viewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';

import { AuthShell } from '../shell/auth-shell';
import { email, form, FormField, minLength, pattern, required } from '@angular/forms/signals';
import { CadastroRequest } from '../shell/auth.models';

interface CadastroFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  standalone: true,
  imports: [LucideArrowRight, AuthShell, FormField, RouterLink],
  selector: 'app-cadastro',
  templateUrl: './cadastro.html',
})
export class Cadastro {
  private readonly tituloPagina = viewChild<ElementRef<HTMLHeadingElement>>('tituloPagina');

  protected readonly modeloFormulario = signal<CadastroFormModel>({ email: '', senha: '' });
  protected readonly formularioCadastro = form(this.modeloFormulario, (modelo) => {
    required(modelo.email, { message: 'Informe seu e-mail.' });
    email(modelo.email, { message: 'Informe um e-mail válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
    pattern(modelo.senha, /(?=.*\d)(?=.*[^a-zA-Z0-9]).+/, {
      message: 'Inclua pelo menos um número e um caractere especial.',
    });
  });

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());
  }

  enviarFormulario(event: SubmitEvent) {
    event.preventDefault();

    const request: CadastroRequest = {
      email: this.modeloFormulario().email,
      senha: this.modeloFormulario().senha,
    };
  }
}
