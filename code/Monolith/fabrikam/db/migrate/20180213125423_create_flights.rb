class CreateFlights < ActiveRecord::Migration[5.1]
  def change
    create_table :flights do |t|
      t.string :duration
      t.string :schedule

      t.timestamps
    end
  end
end
