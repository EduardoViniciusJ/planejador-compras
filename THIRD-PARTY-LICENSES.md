# Licencas de terceiros

Este documento registra as dependencias adicionadas para a exportacao de
relatorios. As versoes estao fixadas no projeto
`PlanejadorCompras.Infrastructure`.

| Pacote | Versao | Uso no projeto | Licenca |
| --- | --- | --- | --- |
| [ClosedXML](https://www.nuget.org/packages/ClosedXML/0.105.0) | 0.105.0 | Geracao de arquivos Excel `.xlsx` | MIT |
| [PDFsharp](https://www.nuget.org/packages/PDFsharp/6.2.4) | 6.2.4 | Renderizacao e gravacao dos arquivos PDF | MIT |
| [PDFsharp-MigraDoc](https://www.nuget.org/packages/PDFsharp-MigraDoc/6.2.4) | 6.2.4 | Modelo de documento, tabelas e diagramacao dos relatorios PDF | MIT |
| [Liberation Sans](https://github.com/liberationfonts/liberation-fonts) | 2.x | Fonte incorporada para renderizacao consistente dos PDFs | SIL Open Font License 1.1 |

Os avisos e textos integrais das licencas acompanham os respectivos pacotes
NuGet e seus repositorios oficiais. Dependencias transitivas conservam suas
proprias licencas e devem ser revisadas novamente sempre que uma versao direta
for atualizada.

O texto integral da licenca da Liberation Sans esta armazenado em
`src/PlanejadorCompras.Infrastructure/Reports/Pdf/Assets/Fonts/LICENSE-LiberationSans.txt`.
