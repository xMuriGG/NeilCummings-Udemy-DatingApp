export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    pageSize: number;
}

export class PaginatedResult<T> {
    result: T;
    pagiantion: Pagination;
}