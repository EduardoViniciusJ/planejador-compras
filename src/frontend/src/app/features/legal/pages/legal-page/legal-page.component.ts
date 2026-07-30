import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { MascotComponent } from '../../../../shared/ui/mascot/mascot.component';

type LegalDocumentId = 'privacy' | 'terms' | 'cookies';

interface LegalSection {
  readonly id: string;
  readonly title: string;
  readonly paragraphs: readonly string[];
  readonly bullets?: readonly string[];
}

interface LegalDocument {
  readonly eyebrow: string;
  readonly title: string;
  readonly introduction: string;
  readonly updatedAt: string;
  readonly highlight: string;
  readonly sections: readonly LegalSection[];
}

const PRIVACY_DOCUMENT: LegalDocument = {
  eyebrow: 'Privacidade',
  title: 'Política de Privacidade',
  introduction:
    'Esta política explica de forma simples como cuidamos das informações usadas no Planejador de Compras.',
  updatedAt: '28 de julho de 2026',
  highlight: 'Seus dados são usados para oferecer a plataforma e manter sua experiência segura.',
  sections: [
    {
      id: 'informacoes',
      title: 'Informações utilizadas',
      paragraphs: [
        'Utilizamos as informações da sua conta necessárias para permitir o acesso à plataforma, além dos dados que você cadastra voluntariamente em listas, itens, fornecedores e cotações.',
      ],
    },
    {
      id: 'uso',
      title: 'Como utilizamos as informações',
      paragraphs: [
        'Essas informações são usadas para organizar suas compras, comparar preços, gerar os recursos solicitados e manter o funcionamento e a segurança do serviço.',
        'Não comercializamos seus dados pessoais. O compartilhamento ocorre somente quando necessário para prestar o serviço ou cumprir uma obrigação legal.',
      ],
    },
    {
      id: 'direitos',
      title: 'Seus direitos',
      paragraphs: [
        'Você pode solicitar informações sobre seus dados, pedir correções, atualizações ou exclusão quando permitido pela legislação aplicável.',
      ],
    },
    {
      id: 'seguranca',
      title: 'Segurança e atualizações',
      paragraphs: [
        'Adotamos medidas para proteger as informações contra acessos indevidos. Esta política poderá ser atualizada para acompanhar melhorias no produto ou mudanças legais.',
      ],
    },
  ],
};

const TERMS_DOCUMENT: LegalDocument = {
  eyebrow: 'Condições de uso',
  title: 'Termos de Uso',
  introduction:
    'Estes termos apresentam as regras básicas para utilizar o Planejador de Compras com segurança e responsabilidade.',
  updatedAt: '28 de julho de 2026',
  highlight:
    'Ao utilizar a plataforma, você concorda com estas condições e com o uso responsável do serviço.',
  sections: [
    {
      id: 'plataforma',
      title: 'Uso da plataforma',
      paragraphs: [
        'O Planejador de Compras ajuda a organizar listas, registrar cotações, comparar valores e apoiar decisões de compra.',
      ],
    },
    {
      id: 'responsabilidades',
      title: 'Responsabilidades do usuário',
      paragraphs: ['Ao utilizar a plataforma, você se compromete a:'],
      bullets: [
        'informar dados corretos e necessários;',
        'manter o acesso à sua conta protegido;',
        'respeitar a legislação e os direitos de terceiros;',
        'não tentar prejudicar ou acessar indevidamente o serviço.',
      ],
    },
    {
      id: 'resultados',
      title: 'Resultados e decisões',
      paragraphs: [
        'Os cálculos e comparações dependem das informações cadastradas. Antes de concluir uma compra, confira preços, quantidades, prazos e demais condições comerciais.',
      ],
    },
    {
      id: 'alteracoes',
      title: 'Disponibilidade e alterações',
      paragraphs: [
        'A plataforma poderá passar por manutenções e melhorias. Estes termos também poderão ser atualizados, mantendo nesta página a versão mais recente.',
      ],
    },
  ],
};

const COOKIES_DOCUMENT: LegalDocument = {
  eyebrow: 'Navegação',
  title: 'Política de Cookies',
  introduction:
    'Esta política explica de maneira resumida como cookies e recursos semelhantes podem apoiar sua navegação.',
  updatedAt: '28 de julho de 2026',
  highlight: 'Usamos esses recursos para permitir o acesso e lembrar preferências da plataforma.',
  sections: [
    {
      id: 'conceito',
      title: 'O que são cookies',
      paragraphs: [
        'Cookies são pequenos arquivos que ajudam sites a funcionar, reconhecer uma sessão e lembrar algumas escolhas feitas durante a navegação.',
      ],
    },
    {
      id: 'finalidade',
      title: 'Como são utilizados',
      paragraphs: [
        'No Planejador de Compras, esses recursos podem ser usados para manter o acesso à conta, preservar preferências e oferecer uma navegação mais consistente.',
      ],
    },
    {
      id: 'controle',
      title: 'Como você pode controlar',
      paragraphs: [
        'Você pode apagar ou bloquear cookies nas configurações do navegador. Algumas funções, como o acesso à área autenticada, podem deixar de funcionar corretamente.',
      ],
    },
    {
      id: 'terceiros',
      title: 'Serviços de terceiros',
      paragraphs: [
        'Recursos externos necessários ao acesso à plataforma podem utilizar tecnologias próprias, de acordo com suas respectivas políticas de privacidade.',
      ],
    },
  ],
};

const LEGAL_DOCUMENTS: Record<LegalDocumentId, LegalDocument> = {
  privacy: PRIVACY_DOCUMENT,
  terms: TERMS_DOCUMENT,
  cookies: COOKIES_DOCUMENT,
};

@Component({
  selector: 'app-legal-page',
  imports: [MascotComponent],
  templateUrl: './legal-page.component.html',
  styleUrl: './legal-page.component.scss',
})
export class LegalPageComponent {
  private readonly route = inject(ActivatedRoute);

  protected readonly document = this.resolveDocument();
  protected readonly relatedDocuments = (
    [
      {
        id: 'privacy',
        label: 'Política de Privacidade',
        href: '/politica-de-privacidade',
      },
      { id: 'terms', label: 'Termos de Uso', href: '/termos-de-uso' },
      { id: 'cookies', label: 'Política de Cookies', href: '/politica-de-cookies' },
    ] as const
  ).filter(({ id }) => id !== this.route.snapshot.data['legalDocument']);

  private resolveDocument(): LegalDocument {
    const documentId = this.route.snapshot.data['legalDocument'] as LegalDocumentId | undefined;
    return documentId ? LEGAL_DOCUMENTS[documentId] : PRIVACY_DOCUMENT;
  }
}
