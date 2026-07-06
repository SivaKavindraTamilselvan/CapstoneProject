import { Signal, signal } from '@angular/core';

export abstract class BasePage {

    protected abstract loadData(): void;

    pageNumber = signal(1);
    pageSize = signal(10);

    abstract totalPages: Signal<number>;

    filterPanelOpen = signal<boolean>(false);
    filterErrorMessage = signal<string | null>(null);
    filterApplied = signal(false);

    protected abstract clearFilterValues(): void;

    showPopup = signal(false);
    selectedId = signal<number | null>(null);
    popupTitle = signal('');
    popupMessage = signal('');
    popupConfirmText = signal('');
    popupButtonClass = signal('');
    titleClass = signal('');


    protected scrollToTop(): void {
        window.scrollTo({
            top: 0,
            left: 0,
            behavior: 'smooth'
        });
    }

    goToPage(page: number): void {

        if (page < 1 || page > this.totalPages()) {
            return;
        }
        this.pageNumber.set(page);
        this.loadData();
        this.scrollToTop();
    }

    nextPage(): void {
        this.goToPage(this.pageNumber() + 1);
    }

    previousPage(): void {
        this.goToPage(this.pageNumber() - 1);
    }

    onPageSizeChanged(size: number): void {
        this.pageSize.set(size);
        this.pageNumber.set(1);
        this.loadData();
    }

    toggleFilterPanel(): void {
        const wasOpen = this.filterPanelOpen();
        this.filterPanelOpen.update((open) => !open);
        if (wasOpen && !this.filterApplied()) {
            this.resetFilters();
        }
    }

    closeFilterPanel(): void {
        this.filterPanelOpen.set(false);
    }

    applyFilters(): void {
        if (this.filterErrorMessage()) {
            return;
        }
        this.pageNumber.set(1);
        this.filterApplied.set(true);
        this.loadData();
        this.closeFilterPanel();
    }

    resetFilters(): void {
        this.filterErrorMessage.set(null);
        this.filterApplied.set(false);
        this.pageNumber.set(1);
        this.clearFilterValues();
        this.loadData();
    }

    openPopup(id: number) {
        this.selectedId.set(id);
        this.showPopup.set(true);
    }

    closePopup() {
        this.showPopup.set(false);
        this.selectedId.set(null);
    }
}