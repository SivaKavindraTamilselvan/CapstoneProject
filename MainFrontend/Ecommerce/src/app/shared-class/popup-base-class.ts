import { signal } from '@angular/core';

export abstract class PopupBase {

    showPopup = signal(false);
    selectedId = signal<number | null>(null);

    popupTitle = signal('');
    popupMessage = signal('');
    popupConfirmText = signal('');
    popupButtonClass = signal('');
    titleClass = signal('');
    loadingText = signal('');

    openPopup(id: number): void {
        this.selectedId.set(id);
        this.showPopup.set(true);
    }

    closePopup(): void {
        this.showPopup.set(false);
        this.selectedId.set(null);

        this.popupTitle.set('');
        this.popupMessage.set('');
        this.popupConfirmText.set('');
        this.popupButtonClass.set('');
        this.titleClass.set('');
    }
}