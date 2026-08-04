export interface ShoppingItemUnitOption {
  readonly value: string;
  readonly label: string;
}

export const SHOPPING_ITEM_UNIT_OPTIONS: readonly ShoppingItemUnitOption[] = [
  { value: 'un', label: 'Unidade (un)' },
  { value: 'cx', label: 'Caixa (cx)' },
  { value: 'pct', label: 'Pacote (pct)' },
  { value: 'kg', label: 'Quilograma (kg)' },
  { value: 'g', label: 'Grama (g)' },
  { value: 'L', label: 'Litro (L)' },
  { value: 'mL', label: 'Mililitro (mL)' },
  { value: 'm', label: 'Metro (m)' },
  { value: 'cm', label: 'Centímetro (cm)' },
  { value: 'par', label: 'Par' },
  { value: 'kit', label: 'Kit' },
  { value: 'rl', label: 'Rolo (rl)' },
];
