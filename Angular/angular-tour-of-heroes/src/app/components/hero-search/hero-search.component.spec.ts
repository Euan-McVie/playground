import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HeroSearchComponent } from './hero-search.component';
import { MatListModule } from '@angular/material/list';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { InitialHeroes } from 'src/app/services/in-memory-data.service';
import { MockProvider } from 'ng-mocks';
import { HeroService } from 'src/app/services/hero.service';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('HeroSearchComponent', () => {
  let component: HeroSearchComponent;
  let fixture: ComponentFixture<HeroSearchComponent>;

  let mockHeroService = MockProvider(HeroService, {
    heroes$: of(InitialHeroes)
  });

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HeroSearchComponent],
      imports: [MatListModule, MatFormFieldModule, FormsModule, MatInputModule, BrowserAnimationsModule],
      providers: [mockHeroService]
    })
      .compileComponents();

    fixture = TestBed.createComponent(HeroSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
