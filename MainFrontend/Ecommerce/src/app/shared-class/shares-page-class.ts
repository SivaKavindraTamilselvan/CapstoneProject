import { Signal, signal } from '@angular/core';
import { PopupBase } from './popup-base-class';

export abstract class BasePage extends PopupBase {

    protected abstract loadData(): void;

    pageNumber = signal(1);
    pageSize = signal(10);

    abstract totalPages: Signal<number>;

    filterPanelOpen = signal<boolean>(false);
    filterErrorMessage = signal<string | null>(null);
    filterApplied = signal(false);

    protected abstract clearFilterValues(): void;

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

    allowOnlyNumbers(event: KeyboardEvent): void {
        const allowedKeys = ['Backspace', 'Delete', 'Tab', 'Escape', 'Enter', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
        if (allowedKeys.includes(event.key) || /^[0-9]$/.test(event.key)) {
            return;
        }
        event.preventDefault();
    }
    allowOnlyDecimals(event: KeyboardEvent): void {
        const allowedKeys = ['Backspace', 'Delete', 'Tab', 'Escape', 'Enter', 'ArrowLeft', 'ArrowRight', 'Home', 'End','.'];
        if (allowedKeys.includes(event.key) || /^[0-9]$/.test(event.key)) {
            return;
        }
        event.preventDefault();
    }
}