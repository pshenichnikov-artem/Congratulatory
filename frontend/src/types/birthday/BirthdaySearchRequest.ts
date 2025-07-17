import type { BirthdayFilterRequest } from './BirthdayFilterRequest'
import type { SortRequest } from '../common/SortRequest'
export interface BirthdaySearchRequest {
  filter: BirthdayFilterRequest
  sort: SortRequest
}