import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Todo, CreateTodoRequest } from './models/todo.model';
import { TodoService } from './services/todo.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatIconModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Todo List';
  todos$: Observable<Todo[]>;
  todoForm: FormGroup;
  isLoading = false;

  private destroy$ = new Subject<void>();

  constructor(
    private todoService: TodoService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private dialog: MatDialog 
  ) {
    this.todos$ = this.todoService.todos$;
    this.todoForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    this.loadTodos();
  }

   ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadTodos(): void {
    this.isLoading = true;
    this.todoService.loadTodos()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.showMessage('Failed to load todos');
        console.error('Error loading todos:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.todoForm.valid) {
      const request: CreateTodoRequest = {
        title: this.todoForm.value.title.trim()
      };

      this.todoService.createTodo(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.todoForm.reset({
            title: ''
          });
          this.todoForm.markAsPristine();
          this.todoForm.markAsUntouched();

          this.showMessage('Todo added successfully!');
        },
        error: (error) => {
          this.showMessage('Failed to add todo');
          console.error('Error creating todo:', error);
        }
      });
    }
  }

  deleteTodo(todo: Todo): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: {
        title: 'Delete Todo',
        message: `Are you sure you want to delete "${todo.title}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        if (result) {
          this.todoService.deleteTodo(todo.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: () => {
                this.showMessage('Todo deleted successfully!');
              },
              error: (error) => {
                this.showMessage('Failed to delete todo');
                console.error('Error deleting todo:', error);
              }
            });
        }
      });
  }

  private showMessage(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom'
    });
  }

  get titleControl() {
    return this.todoForm.get('title');
  }

  trackByTodoId(index: number, todo: Todo): number {
    return todo.id;
  }
}