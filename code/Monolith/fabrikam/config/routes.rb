Rails.application.routes.draw do
  resources :frights
  resources :flights
  resources :drones
  resources :users
  # For details on the DSL available within this file, see http://guides.rubyonrails.org/routing.html
end
