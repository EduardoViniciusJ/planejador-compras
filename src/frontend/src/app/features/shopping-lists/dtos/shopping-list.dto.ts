export interface ShoppingListRequestDto {
  readonly name: string;
  readonly description: string | null;
}

export interface ShoppingListResponseDto {
  readonly id: string;
  readonly name: string;
  readonly description: string | null;
  readonly createdAt: string;
}

export interface ShoppingListOverviewDto extends ShoppingListResponseDto {
  readonly itemCount: number;
  readonly quotedItemCount: number;
  readonly estimatedTotal: number;
}

export interface ShoppingListsSummaryDto {
  readonly totalLists: number;
  readonly draftLists: number;
  readonly awaitingQuotesLists: number;
  readonly readyForEqualizationLists: number;
  readonly totalEstimated: number;
}

export interface ShoppingListsOverviewResponseDto {
  readonly summary: ShoppingListsSummaryDto;
  readonly lists: readonly ShoppingListOverviewDto[];
}
