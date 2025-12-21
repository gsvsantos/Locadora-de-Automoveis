import { Component, ElementRef, HostListener, inject, ViewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  Observable,
  of,
  debounceTime,
  distinctUntilChanged,
  switchMap,
  finalize,
  BehaviorSubject,
  combineLatest,
  map,
  shareReplay,
} from 'rxjs';
import { GlobalSearch } from '../../../models/search.models';
import { SearchService } from '../../../services/search.service';
import { AsyncPipe, CommonModule } from '@angular/common';
import { TranslocoModule } from '@jsverse/transloco';

@Component({
  selector: 'app-multi-search',
  imports: [AsyncPipe, CommonModule, TranslocoModule, ReactiveFormsModule],
  templateUrl: './multi-search.component.html',
  styleUrl: './multi-search.component.scss',
})
export class MultiSearchComponent {
  private readonly searchService = inject(SearchService);
  private readonly router = inject(Router);

  @ViewChild('searchContainer') public searchContainer?: ElementRef<HTMLDivElement>;

  private readonly minimumLength: number = 2;
  protected readonly searchControl: FormControl<string> = new FormControl<string>('', {
    nonNullable: true,
  });

  private readonly isLoadingSubject = new BehaviorSubject<boolean>(false);
  protected readonly isLoading$: Observable<boolean> = this.isLoadingSubject.asObservable();

  protected readonly queryText$: Observable<string> = this.searchControl.valueChanges.pipe(
    debounceTime(300),
    distinctUntilChanged(),
    map((value: string) => value.trim()),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly isQueryTooShort$: Observable<boolean> = this.queryText$.pipe(
    map((text: string) => text.length > 0 && text.length < this.minimumLength),
  );

  protected readonly results$: Observable<GlobalSearch[]> = this.queryText$.pipe(
    switchMap((text: string) => {
      const canSearch = text.length >= this.minimumLength;

      if (!canSearch) {
        this.isLoadingSubject.next(false);
        return of([] as GlobalSearch[]);
      }

      this.isLoadingSubject.next(true);

      return this.searchService
        .globalSearch(text)
        .pipe(finalize(() => this.isLoadingSubject.next(false)));
    }),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  protected readonly showResults$: Observable<boolean> = combineLatest([
    this.results$,
    this.isQueryTooShort$,
    this.queryText$,
  ]).pipe(
    map(
      ([items, tooShort, text]) =>
        text.length >= this.minimumLength && !tooShort && items.length > 0,
    ),
  );

  protected onSelectResult(route: string): void {
    this.searchControl.setValue('', { emitEvent: true });
    void this.router.navigateByUrl(route);
  }

  @HostListener('document:click', ['$event'])
  protected onDocumentClick(event: MouseEvent): void {
    const searchContainerElement: HTMLDivElement | undefined = this.searchContainer?.nativeElement;

    if (!searchContainerElement) {
      return;
    }

    const targetNode: Node | null = event.target as Node | null;

    if (targetNode && !searchContainerElement.contains(targetNode)) {
      this.searchControl.setValue('', { emitEvent: true });
    }
  }

  protected getIcon(type: string): string {
    switch (type) {
      case 'Employee':
        return 'badge';
      case 'Vehicle':
        return 'directions_car';
      case 'Client':
        return 'person';
      case 'Driver':
        return 'local_taxi';
      case 'Coupon':
        return 'local_offer';
      case 'Rental':
        return 'key';
      case 'Group':
        return 'bookmarks';
      default:
        return 'search';
    }
  }
}
