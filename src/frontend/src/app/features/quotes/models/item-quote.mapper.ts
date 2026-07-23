import { ItemQuoteResponseDto, UserItemQuoteDto } from '../dtos/item-quote.dto';
import { ItemQuote, UserItemQuote } from './item-quote.model';

export function mapItemQuote(dto: ItemQuoteResponseDto): ItemQuote {
  return { ...dto, createdAt: new Date(dto.createdAt) };
}

export function mapUserItemQuote(dto: UserItemQuoteDto): UserItemQuote {
  return { ...dto, createdAt: new Date(dto.createdAt), totalPrice: dto.unitPrice * dto.quantity };
}
