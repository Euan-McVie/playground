import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ngMocks, MockComponent, MockProvider } from 'ng-mocks';
import { HeroesComponent } from './heroes.component';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeroDetailComponent } from '../hero-detail/hero-detail.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { InitialHeroes } from 'src/app/services/in-memory-data.service';
import { MatIconModule } from '@angular/material/icon';
import { HeroService } from 'src/app/services/hero.service';
import { of } from 'rxjs';

describe('HeroesComponent', () => {
  let heroComponent: HeroesComponent;
  let heroDetailsComponent: HeroDetailComponent;
  let componentElement: HTMLElement;
  let fixture: ComponentFixture<HeroesComponent>;

  let mockHeroService = MockProvider(HeroService, {
    heroes$: of(InitialHeroes),
    getHero: (id: number) => of(InitialHeroes.find(x => x.id === id)!)
  });

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HeroesComponent, MockComponent(HeroDetailComponent)],
      imports: [
        BrowserAnimationsModule,
        MatListModule,
        MatSidenavModule,
        MatFormFieldModule,
        ActivatedRoute,
        RouterTestingModule,
        MatIconModule
      ],
      providers: [mockHeroService]
    })
      .compileComponents();



    fixture = TestBed.createComponent(HeroesComponent);
    heroComponent = fixture.componentInstance;
    componentElement = fixture.nativeElement as HTMLElement;
    heroDetailsComponent = ngMocks.find<HeroDetailComponent>('app-hero-detail').componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(heroComponent).toBeTruthy();
  });

  it('should render title', () => {
    expect(componentElement.querySelector('mat-action-list h2')?.textContent).toEqual("My Heroes");
  });

  it('should render hero list', () => {
    const actualNames = Array.from(componentElement.querySelectorAll('mat-list-item button.heroName'))
      .map(x => x.textContent);
    const expectedNames = InitialHeroes.map(x => x.name);
    expect(actualNames).toEqual(expectedNames);
  });

  it('should open hero details when hero selected', () => {
    // Arrange
    const heroButton = componentElement.querySelector('mat-action-list button.heroName') as HTMLButtonElement;

    // Act
    heroButton.click();
    fixture.detectChanges();

    // Assert
    expect(heroDetailsComponent.heroId).toEqual(InitialHeroes[0].id);
  });

});
