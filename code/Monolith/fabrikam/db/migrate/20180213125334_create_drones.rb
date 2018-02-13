class CreateDrones < ActiveRecord::Migration[5.1]
  def change
    create_table :drones do |t|
      t.string :state
      t.boolean :has_load
      t.belongs_to :flight, index: true

      t.timestamps
    end
  end
end
