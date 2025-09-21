import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Todo, CreateTodoRequest } from '../models/todo.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = `${environment.apiBaseUrl}/api/todos`;
  private todosSubject = new BehaviorSubject<Todo[]>([]);
  public todos$ = this.todosSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.apiUrl).pipe(
      tap(todos => this.todosSubject.next(todos))
    );
  }

  createTodo(request: CreateTodoRequest): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, request).pipe(
      tap(newTodo => {
        const currentTodos = this.todosSubject.value;
        this.todosSubject.next([...currentTodos, newTodo]);
      })
    );
  }

  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        const currentTodos = this.todosSubject.value;
        this.todosSubject.next(currentTodos.filter(todo => todo.id !== id));
      })
    );
  }

  getTodos(): Todo[] {
    return this.todosSubject.value;
  }
}