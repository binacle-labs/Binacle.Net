import {PagedResponse, PageQuery} from "binacle-net-service-client";

// The state behind a paged table. The page owns the rows; this owns where in the list they came from.
export class Paging {
	page = 1;
	pageSize = 50;
	allowDeleted = false;
	total = 0;
	totalPages = 0;

	query(): PageQuery {
		return {page: this.page, pageSize: this.pageSize, allowDeleted: this.allowDeleted};
	}

	take(response: PagedResponse<unknown>): void {
		this.page = response.page;
		this.pageSize = response.pageSize;
		this.total = response.total;
		this.totalPages = response.totalPages;
	}

	get hasPrevious(): boolean {
		return this.page > 1;
	}

	get hasNext(): boolean {
		return this.page < this.totalPages;
	}
}
