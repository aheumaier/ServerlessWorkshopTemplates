class CreateFrights < ActiveRecord::Migration[5.1]
  def change
    create_table :frights do |t|
      t.string :name
      t.belongs_to :flight, index: true

      t.timestamps
    end
  end
end
