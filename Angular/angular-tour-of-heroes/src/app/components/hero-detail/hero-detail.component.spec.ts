import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HeroDetailComponent } from './hero-detail.component';
import { Hero } from 'src/app/models/hero';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSidenavModule } from '@angular/material/sidenav';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('HeroDetailComponent', () => {
  let component: HeroDetailComponent;
  let componentElement: HTMLElement;
  let fixture: ComponentFixture<HeroDetailComponent>;

  const initial: Hero = { id: 0, name: 'Windstorm' };
  const newName = "Hydro";

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HeroDetailComponent],
      imports: [
        BrowserAnimationsModule,
        MatCardModule,
        MatFormFieldModule,
        MatSidenavModule,
        FormsModule,
        MatInputModule,
        HttpClientTestingModule
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(HeroDetailComponent);
    component = fixture.componentInstance;
    component.hero = initial;
    componentElement = fixture.nativeElement as HTMLElement;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it(`should have as name '${initial.name}'`, () => {
    expect(component.hero?.name).toEqual(initial.name);
  });

  it('should render name', () => {
    const nameInput = componentElement.querySelector('#name') as HTMLInputElement;
    expect(nameInput?.value).toEqual(initial.name);
  });

  it('should render details title', () => {
    const titleElement = componentElement.querySelector('#details mat-card-title') as HTMLElement;
    expect(titleElement?.textContent).toEqual(`${initial.name.toUpperCase()} Details`);
  });

  it('should update name model when new name entered', () => {
    // Arrange
    const nameInput = componentElement.querySelector('#name') as HTMLInputElement;

    // Act
    nameInput.value = newName;
    nameInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    // Assert
    expect(component.hero?.name).toEqual(newName);
  });

  it('should update details title when name changes', () => {
    // Arrange
    const nameInput = componentElement.querySelector('#name') as HTMLInputElement;
    const titleElement = componentElement.querySelector('#details mat-card-title') as HTMLElement;

    // Act
    nameInput.value = newName;
    nameInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    // Assert
    expect(titleElement?.textContent).toEqual(`${newName.toUpperCase()} Details`);
  });
});
